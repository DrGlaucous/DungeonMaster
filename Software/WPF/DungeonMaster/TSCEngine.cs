using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Timers;
using Microsoft.VisualBasic;
using System.IO;
using System.Reflection;
using System.Windows.Input;


namespace DungeonMaster
{

    enum EngineState
    {
        Idle,
        Running,
        Waiting,
    }


    internal class TSCObject {
        public string OPCode = String.Empty;
        public List<string> arguments = [];

        //returns the number of args exepcted from the opcode
        public static bool GetArgCount(string opcode, out int int_arg_ct, out int str_arg_ct)
        {
            switch (opcode) {
                case "TGT":
                    {
                        int_arg_ct = 1;
                        str_arg_ct = 0;
                        break;
                    }
                case "SLT":
                    {
                        int_arg_ct = 3;
                        str_arg_ct = 0;
                        break;
                    }
                case "STS":
                    {
                        int_arg_ct = 2;
                        str_arg_ct = 0;
                        break;
                    }
                case "SLC":
                    {
                        int_arg_ct = 3;
                        str_arg_ct = 0;
                        break;
                    }
                case "SLR":
                    {
                        int_arg_ct = 0;
                        str_arg_ct = 0;
                        break;
                    }
                case "SLS":
                    {
                        int_arg_ct = 6;
                        str_arg_ct = 0;
                        break;
                    }
                case "WAI":
                    {
                        int_arg_ct = 2;
                        str_arg_ct = 0;
                        break;
                    }
                case "PSH":
                    {
                        int_arg_ct = 1;
                        str_arg_ct = 0;
                        break;
                    }
                case "POP":
                    {
                        int_arg_ct = 0;
                        str_arg_ct = 0;
                        break;
                    }
                case "KEY":
                    {
                        int_arg_ct = 0;
                        str_arg_ct = 0;
                        break;
                    }
                case "FRE":
                    {
                        int_arg_ct = 0;
                        str_arg_ct = 0;
                        break;
                    }
                case "EVE":
                    {
                        int_arg_ct = 1;
                        str_arg_ct = 0;
                        break;
                    }
                default:
                    {
                        int_arg_ct = 0;
                        str_arg_ct = 0;
                        return false;
                    }
            }

            return true;

        }


        //re-assembles code and arguments into a TSC command for sending over the COM port
        public string ReassembleCmd() {

            string command = "<" + OPCode;

            for (int i = 0; i < arguments.Count; ++i) { 
                command += arguments[i];

                if (i < arguments.Count - 1) {
                    command += ":";
                }
            }
            return command;
        }

    }
    internal class TSCEvent {
        public int Event = 0;
        public List<TSCObject> Commands = [];

    }

    internal class TSCState
    {
        public EngineState state = EngineState.Idle;
        public bool key = false; //should we wait for the engine state to be idle before running the next event?

        int event_no = 0; //what event to find
        int event_start_index = 0; //where to start looking for the event number in the list

        int command_start_index = 0; //where to start running commands in the event



    }
    
    internal class TSCEngine
    {

        private System.Timers.Timer wait_timer = new();

        //holds the events to be run
        private List<TSCEvent> event_list = [];


        //what events to run next
        private Queue<int> event_queue = new();
        //used with <PSH and <POP to return to previous event + location
        private Stack<(int event_start_index, int command_start_index)> event_stack = new();
        //what state the TSC engine is in right now
        private EngineState state = EngineState.Idle;
        //should we wait for the engine state to be idle before running the next event?
        public bool key = false;


        //writes to output functions
        public delegate void OnWriteOutHandler(string output);
        public event OnWriteOutHandler? SendCommandHandler; //sends output to terminal + over serial
        public event OnWriteOutHandler? SendMessageHandler; //sends output to terminal only (for viewer messages)

        //constructor
        public TSCEngine()
        {

            wait_timer.AutoReset = false;
            wait_timer.Enabled = true;
            wait_timer.Elapsed += ResumeFromWait;
            wait_timer.Stop();

        }


        //loads the script from a path location
        public bool LoadScript(string path)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                var contents = reader.ReadToEnd();
                reader.Close();
                ParseScript(contents);
            }
            catch { return false; }

            return true;
        }

        //converts a string TSC script into an array of event objects internally, erases old event list!
        public void ParseScript(string script) {


            //string test_string = "#3533OKvanilla\n\n<CMD<CM2random_text<APP0000:1111<GRN2222:3333\n#3563nicevanilla\ngood_practice";
            //script = test_string;

            //filter out comments
            //see <https://stackoverflow.com/questions/12089630/regular-expression-for-filter-c-comments>
            script = Regex.Replace(script, "/(\\/\\*(?<multiline>[\\s\\S]*?)\\*\\/)|(\\/\\/(?<singleline>[\\s\\S]*?)[\\n\\r]?$)/mg", "");

            //split by events (first index always contains garbage data or an empty string)
            string[] split_strings = Regex.Split(script, "(?=#....)");

            //wipe current cached TSC
            event_list.Clear();

            //reset running state
            ClearState();

            //parse each event and add it to the event list
            for (int i = 1; i < split_strings.Length; ++i) {
                //foreach (var istring in split_strings) {
                var istring = split_strings[i];

                if (istring == String.Empty) {
                    continue;
                }

                //trim off extra stuff after event number, so something like #1234abcd\n will be #1234\n
                string event_trimmed = Regex.Replace(istring, "(?<=#....).*", "");

                //get event number
                int event_num = GetNumberFromString(event_trimmed[1..]);

                TSCEvent tsc_event = new();
                tsc_event.Event = event_num;

                tsc_event.Commands = ParseCommandArray(ref istring);

                event_list.Add(tsc_event);
            }
        }

        //parses a command array (minus the event number), so something like "<KEY<MSGHi<EVE0000<END"
        private static List<TSCObject> ParseCommandArray(ref string istring)
        {

            var object_list = new List<TSCObject>();

            //split by opcode
            string[] command_array = Regex.Split(istring, "(?=<...)");
            for(int i = 0; i < command_array.Length; ++i)
            {
                var command = command_array[i];

                //typically the first entry will either be empty string or contain non-command lead-ins
                if (command.Length < 1 || command[0] != '<')
                {
                    continue;
                }

                TSCObject obj = new();

                //get opcode
                obj.OPCode = command[1..4]; //note: range operator 1..4 is [1,4)

                //get list of raw args (determining how to enterperet each one will happen when we actually run the script)
                if (command.Length > 4)
                {

                    //break by delimiter
                    string[] argument_array = Regex.Split(command[4..], ":");
                    foreach (var arg in argument_array)
                    {
                        //first entry should not be discarded here
                        obj.arguments.Add(arg);
                    }
                }

                object_list.Add(obj);

            }

            //forcibly add <END command to protect against infinite loops
            //intentional loops can still be made with <EVE
            TSCObject endobj = new() { OPCode = "END" };
            object_list.Add(endobj);

            return object_list;
        }

        private static int GetNumberFromString(string input) {
            //string must have at least 4 digits (only the first 4 are considered)
            if (input.Length < 4) {
                return 0;
            }

            var number = 0;

            //intentional no-underflow checking (allows for OOB and negative values, which could be useful)
            number += (input[0] - '0') * 1000;
            number += (input[1] - '0') * 100;
            number += (input[2] - '0') * 10;
            number += input[3] - '0';

            return number;
        }


        //drops whatever its doing and begins running this next event from the internal list. if the event doesn't exist, the state is set to idle
        //note: does NOT clear stack or queue, so bad things may happen if used without going through PushEvent
        private void FindRunEvent(
            int event_no, //what event to find
            int event_start_index, //where to start looking for the event number in the list
            int command_start_index //where to start running commands in the event
            )
        { 
            state = EngineState.Running;
            while (state == EngineState.Running)
            {  
                for (int i = event_start_index; i < event_list.Count; ++i)
                {
                    //event matches, run it
                    if (event_list[i].Event == event_no)
                    {
                        //for each sequential command in the command list
                        for (int j = command_start_index; j < event_list[i].Commands.Count; ++j)
                        {
                            var command = event_list[i].Commands[j];
                            bool seeking = false;
                            switch (command.OPCode)
                            {
                                //case "TGT":
                                //case "SLT":
                                //case "STS":
                                //case "SLC":
                                //case "SLR":
                                //case "SLS":
                                default:
                                    {
                                        //send these commands out to the connected device
                                        string output = command.ReassembleCmd();
                                        SendCommandHandler?.Invoke(output);
                                        break;
                                    }
                                case "MSG":
                                    {
                                        if (command.arguments.Count > 0)
                                        {
                                            //pipe out directly
                                            SendMessageHandler?.Invoke(command.arguments[0]);
                                        }
                                        break;
                                    }
                                case "WAI":
                                    {
                                        try
                                        {
                                            var seconds = GetNumberFromString(command.arguments[0]);
                                            var millis = GetNumberFromString(command.arguments[1]);


                                            event_stack.Push((i, j + 1));
                                            state = EngineState.Waiting;

                                            //break out
                                            event_start_index = 0;
                                            seeking = true;

                                            //start callback timer
                                            if (millis + seconds * 1000 > 0)
                                            {
                                                wait_timer.Interval = millis + seconds * 1000;
                                                wait_timer.Start();
                                            }
                                            else
                                            {
                                                seeking = false; //don't try to wait if negative
                                            }
                                        }
                                        catch { }
                                        break;
                                    }
                                case "PSH":
                                    {
                                        //jump to next event if we have a valid event argument
                                        try
                                        {
                                            //target sub-command
                                            event_no = GetNumberFromString(command.arguments[0]);
                                            command_start_index = 0;
                                            event_start_index = 0;

                                            event_stack.Push((i, j + 1)); //keep track of return location
                                                                      //seek to find new event
                                            seeking = true;
                                        }
                                        catch { }
                                        break;
                                    }
                                case "POP":
                                    {
                                        try
                                        {
                                            var prev_event = event_stack.Pop();
                                            event_start_index = prev_event.event_start_index;
                                            event_no = event_list[event_start_index].Event;
                                            command_start_index = prev_event.command_start_index;
                                            seeking = true;
                                        }
                                        catch { }
                                        break;
                                    }
                                case "KEY":
                                    {
                                        key = true;
                                        break;
                                    }
                                case "FRE":
                                    {
                                        key = false;
                                        break;
                                    }
                                case "EVE":
                                    {
                                        //jump to next event if we have a valid event argument
                                        try
                                        {
                                            event_no = GetNumberFromString(command.arguments[0]);
                                            command_start_index = 0;
                                            event_start_index = 0;
                                            seeking = true;
                                        }
                                        catch { }

                                        break;
                                    }
                                case "END":
                                    {
                                        //no need to clear stacks or queues here since we do that when we start a new event

                                        //set this to break out of this event list
                                        seeking = true;

                                        //break out of while loop
                                        state = EngineState.Idle;
                                        break;
                                    }

                            }



                            //stop running this event if we've entered "seeking" mode
                            if (seeking)
                            {
                                break;
                            }

                        }

                        break; //found the event, break out (state should have been set by the event)

                    }
                    //catch event-not-found errors (if we've looked at the last event and haven't run it, simply return to idleing)
                    else if (i == event_list.Count - 1)
                    {
                        //return to idle state
                        state = EngineState.Idle;
                    }

                }
            }
        }


        //parses the command sent from the terminal and sets event -1 to it, running it next
        //debug event occupies slot -1, FYI
        public void RunEventDebug(string debug_string)
        {
            TSCEvent debug_event = new();

            debug_event.Event = -1;
            debug_event.Commands = ParseCommandArray(ref debug_string);

            //checks for existing "-1" entry in event list, replace it if found
            var found_event = false;
            for (int i = 0; i < event_list.Count; ++i)
            {
                if (event_list[i].Event == -1) {
                    event_list[i] = debug_event;
                    found_event = true;
                }
            }
            //push the event anew
            if (!found_event)
            {
                event_list.Add(debug_event);
            }

            ClearState();
            PushEvent(-1);
            
        }

        //run events until queue is empty
        private void DrainEvents()
        {
            //if not keyed or engine is idleing, begin the event immediately
            while ((!key || state == EngineState.Idle) && event_queue.Count > 0)
            {
                PopEvent();
            }
        }


        //push event to the event queue to be run when the earlier events finish, and starts it if it can
        public void PushEvent(int event_no)
        {
            event_queue.Enqueue(event_no);

            DrainEvents();
        }

        //checks for an event in the queue and runs it, if queue is empty, it sets the TSC state to idle
        private void PopEvent() {

            event_stack.Clear(); //if we were in the middle of a <PSH subroutine, this makes sure we don't "pop" back when we shouldn't
            wait_timer.Stop(); //make sure this isn't inturrupted

            if (event_queue.Count > 0)
            {
                var next_event = event_queue.Dequeue();
                FindRunEvent(next_event, 0, 0);
            }
            else
            {
                state = EngineState.Idle;
            }
            
        }

        //clears the queue, stack, and sets the engine to idle
        private void ClearState() {
            state = EngineState.Idle;
            event_queue.Clear();
            event_stack.Clear();
        }

        private void ResumeFromWait(Object? source, ElapsedEventArgs e)
        {
            //wait timer should have self-halted
            if (event_stack.Count > 0)
            {
                //run starting where we left off.
                var (event_index, cmd_no) = event_stack.Pop();
                FindRunEvent(event_list[event_index].Event, event_index, cmd_no);
            }

            //try to empty the remaining queue
            DrainEvents();

        }
    
    
    }


    internal class ResponseEngine
    {
        //matching definitions for these are in "configuration.h" as part of the embedded firmware
        public enum DeviceType {
            Dongle = 0,
            JudgeController = 1,
            Box = 2,
        }

        public enum ResponseType {
            PacketGetOk = 0,
            ButtonStatus = 1,
        }
        public enum ButtonId {
            StartMatch = 0,
            PausePlay = 1,
            EndMatch = 2,
            Stopwatch = 3,
            RedWins = 4,
            BlueWins = 5,
            RedReady = 6,
            BlueReady = 7,
            ArenaDoor = 8,
        }


        public delegate void OnRunEventHandler(int event_no);
        public event OnRunEventHandler? RunEventHandler; //sends output to TSC engine

        //currently parses one input at a time
        public void ParseResponse(string input)
        {
            //test
            //input = "<1:13:1:12 O\n";

            string[] split_strings = Regex.Split(input, "[<:]");

            //all responses should have 4 parts (+1 for anything that comes before the '<')
            if (split_strings.Length < 5)
            {
                return;
            }

            DeviceType device_type = (DeviceType)Int32.Parse(split_strings[1]);
            int device_id = Int32.Parse(split_strings[2]);
            ResponseType response_type = (ResponseType)Int32.Parse(split_strings[3]);
            string response_data = split_strings[4];

            switch (response_type)
            {
                default: { break; }
                case ResponseType.ButtonStatus:
                    {
                        string[] split_response = Regex.Split(response_data, " ");
                        int button_id = Int32.Parse(split_response[0]);
                        bool status = split_response[1][0] == 'O';

                        //run event based on button number + status "on" events are 1000 range, "off" events are 2000 range
                        int event_num = 1000 * (status? 1 : 2) + button_id;
                        RunEventHandler?.Invoke(event_num);

                        break;
                    }
            }

        }
    }

}
