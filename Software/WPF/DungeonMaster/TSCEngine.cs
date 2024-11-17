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
using System.Windows.Shapes;



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
        public event StringDelegate? SendCommandHandler; //sends output to terminal + over serial
        public event StringDelegate? SendMessageHandler; //sends output to terminal only (for viewer messages)

        //used to send new commands to the scoreboard
        public event ScoreboardActionDelegate? ScoreboardControlHandler;

        //holds bit flags, valid range is between 0000 and 7999
        private byte[] flag_arr = new byte[1000];


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
            var raw_pattern = @"(\/\*[\S\s]*?\*\/)|\/\/[\S\s]*?[\n\r]";
            script = Regex.Replace(script, raw_pattern, "");

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
                //catch unloaded events
                if (event_list.Count == 0) {
                    state = EngineState.Idle;
                }
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
                                default: //forwards the command to the dongle
                                    {
                                        //send these commands out to the connected device
                                        string output = command.ReassembleCmd();
                                        SendCommandHandler?.Invoke(output);
                                        break;
                                    }
                                case "MSG": //puts a message on the terminal
                                    {
                                        if (command.arguments.Count > 0)
                                        {
                                            //pipe out directly
                                            SendMessageHandler?.Invoke(command.arguments[0]);
                                        }
                                        break;
                                    }
                                case "WAI": //waits xxxx seconds and yyyy milliseconds
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
                                case "PSH": //pushes a subroutine to the call stack
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
                                case "POP": //returns to parent subroutine
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
                                case "KEY": //forces all incoming commands to wait until this one is finished before executing
                                    {
                                        key = true;
                                        break;
                                    }
                                case "FRE": //allows commands to execute as soon as this one is finished OR it enters a "wait" state
                                    {
                                        key = false;
                                        break;
                                    }
                                case "EVE": //goto event
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
                                case "END": //end event and return to idle mode (good practice to add, but is also manually appended to all commands)
                                    {
                                        //no need to clear stacks or queues here since we do that when we start a new event

                                        //set this to break out of this event list
                                        seeking = true;

                                        //break out of while loop
                                        state = EngineState.Idle;
                                        break;
                                    }
                                case "FL+": //FL+ xxxx, sets flag xxxx to 1
                                    {
                                        try
                                        {
                                            var flag_no = GetNumberFromString(command.arguments[0]);
                                            SetFlag(flag_no, true);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "FL-":  //FL- xxxx, sets flag xxxx to 0
                                    {
                                        try
                                        {
                                            var flag_no = GetNumberFromString(command.arguments[0]);
                                            SetFlag(flag_no, false);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "FLJ": //FLJxxxx:yyyy, jumps to yyyy if flag xxxx is true
                                    {
                                        try
                                        {
                                            var flag_no = GetNumberFromString(command.arguments[0]);

                                            if (GetFlag(flag_no)) {
                                                event_no = GetNumberFromString(command.arguments[1]);
                                                command_start_index = 0;
                                                event_start_index = 0;
                                                seeking = true;
                                            }
                                        }
                                        catch { }

                                        break;
                                    }
                                case "FNJ": //FNJxxxx:yyyy, jumps to yyyy if flag xxxx is false
                                    {
                                        try
                                        {
                                            var flag_no = GetNumberFromString(command.arguments[0]);

                                            if (!GetFlag(flag_no))
                                            {
                                                event_no = GetNumberFromString(command.arguments[1]);
                                                command_start_index = 0;
                                                event_start_index = 0;
                                                seeking = true;
                                            }
                                        }
                                        catch { }

                                        break;
                                    }
                                case "FLC": //resets all flags to state "0"
                                    {
                                        ClearFlags();
                                        break;
                                    }
                                case "SWG": //StopWatch Go
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.StartStopwatch);
                                        break;
                                    }
                                case "SWH": //StopWatch Halt
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.StopStopwatch);
                                        break;
                                    }
                                case "SWR": //StopWatch Reset
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.ResetStopwatch);
                                        break;
                                    }
                                
                                
                                case "TIG": //TImer Go
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.StartMain);
                                        break;
                                    }
                                case "TIH": //TImer Halt
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.StopMain);
                                        break;
                                    }
                                case "TIA": //TImer Add <TIAxxxx:yyyy add xxxx seconds and yyyy milliseconds to the current time 
                                    {
                                        try
                                        {
                                            int s = GetNumberFromString(command.arguments[0]);
                                            int ms = GetNumberFromString(command.arguments[1]);
                                            var total = TimeSpan.FromMilliseconds((s * 1000 + ms));
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.AddTimeMain, total);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "TIS": //TImer Sub <TISxxxx:yyyy subtract xxxx seconds and yyyy milliseconds from the current time 
                                    {
                                        try
                                        {
                                            int s = GetNumberFromString(command.arguments[0]);
                                            int ms = GetNumberFromString(command.arguments[1]);
                                            var total = TimeSpan.FromSeconds(-(s * 1000 + ms));
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.AddTimeMain, total);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "TIR": //TImer Reset, reset the main scoreboard timer to 0
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.ResetMain);
                                        break;
                                    }
                                case "TEU": //Timer Event pUsh <TEUxxxx:yyyy:zzzz set event xxxx to be run when the timer hits yyyy seconds, zzzz ms
                                    {
                                        try
                                        {
                                            int event_num = GetNumberFromString(command.arguments[0]);
                                            int s = GetNumberFromString(command.arguments[1]);
                                            int ms = GetNumberFromString(command.arguments[2]);
                                            var total = TimeSpan.FromMilliseconds(s * 1000 + ms);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.AddTimerEvent, event_num, total);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "TEO": //Timer Event pOp <TEOxxxx removes event xxxx from the list of events to execute
                                    {
                                        try
                                        {
                                            int event_num = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.RemoveTimerEvent, event_num);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "TEC": //Timer Event Clear <TEC removes all events from the list of events to execute
                                    {
                                        try
                                        {
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.ClearTimerEvent);
                                        }
                                        catch { }
                                        break;
                                    }

                                case "AVL": //Audio Video Load, <AVLxxxx:string_arg$ loads an audio clip or video clip on media buffer xxxx [0 or 1] to be played with AVY
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            string path = Regex.Split(command.arguments[1], "\\$")[0]; //trim potential '$' delimiter from the string arg

                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.LoadMedia, player_idx, path);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "AVU": //Audio Video Unload, un-loads the media buffer on xxxx, used to conserve memory when not in use (automatically called before AVL)
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.ShowMedia, player_idx);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "AVH": //Audio Video Hide, <AVHxxxx, hides the media buffer xxxx, but doesn't stop it
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.HideMedia, player_idx);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "AVV": //Audio Video Visible, un-hides the media buffer xxxx
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.ShowMedia, player_idx);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "AVP": //Audio Video Pause, pauses media without resetting it
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.PauseMedia, player_idx);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "AVY": //Audio Video plaY, starts media playback
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.PlayMedia, player_idx);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "AVS": //Audio Video Stop, stops media playback and resets it
                                    {
                                        try
                                        {
                                            var player_idx = GetNumberFromString(command.arguments[0]);
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.StopMedia, player_idx);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "IMS": //IMage Show: <IMSpath_to_image$, loads the image from this path onto the screen
                                    {
                                        try
                                        {
                                            string path = Regex.Split(command.arguments[0], "\\$")[0]; //trim potential '$' delimiter from the string arg
                                            ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.ShowImage, path);
                                        }
                                        catch { }
                                        break;
                                    }
                                case "IMH": //IMage Hide: <IMH, hides the overlay image
                                    {
                                        ScoreboardControlHandler?.Invoke(Scoreboard.ScoreboardAction.HideImage);
                                        break;
                                    }
                                case "TRA": //TRAnsition tsc script <TRA[event number]:[path_to_script$]
                                    {
                                        try
                                        {
                                            int try_event_no = GetNumberFromString(command.arguments[0]);
                                            string path = Regex.Split(command.arguments[1], "\\$")[0]; //trim potential '$' delimiter from the string arg
                                            //if we've successfully loaded this stage, go to the next event
                                            if (LoadScript(path))
                                            {
                                                //run the next event
                                                event_no = try_event_no;
                                                command_start_index = 0;
                                                event_start_index = 0;
                                                seeking = true;
                                                state = EngineState.Running;
                                            }
                                            else
                                            {
                                                //run "END"

                                                //set this to break out of this event list
                                                seeking = true;

                                                //break out of while loop
                                                state = EngineState.Idle;
                                            }
                                        }
                                        catch { }
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

        //works just like ResumeFromWait except it can be called from an outside inturrupt (for things like when we wait for the media to load)
        //potential problem if wait times out and we enter another wait when this is called (it will cut that wait short)
        public void CutWait() {

            //if the timer has timed out, don't try to run this
            if (!wait_timer.Enabled) { return; }

            //stop the timer running and run the event now instead
            wait_timer.Stop();

            if (event_stack.Count > 0)
            {
                //run starting where we left off.
                var (event_index, cmd_no) = event_stack.Pop();
                FindRunEvent(event_list[event_index].Event, event_index, cmd_no);
            }

            //try to empty the remaining queue
            DrainEvents();
        }

        private bool SetFlag(int flag_no, bool state)
        {
            int slot_no = flag_no / (sizeof(byte) * 8);
            int shift_no = flag_no % (sizeof(byte) * 8);

            //OOB protection
            if (flag_no < 0 || slot_no >= flag_arr.Length)
                return false;

            if (state)
            {
                flag_arr[slot_no] |= (byte)(1 << shift_no); //set
            }
            else
            {
                flag_arr[slot_no] &= (byte)~(1 << shift_no); //unset
            }


            return true;
        }

        private bool GetFlag(int flag_no)
        {
            int slot_no = flag_no / (sizeof(byte) * 8);
            int shift_no = flag_no % (sizeof(byte) * 8);

            //OOB protection
            if (flag_no < 0 || slot_no >= flag_arr.Length)
                return false;

            return (flag_arr[slot_no] & (byte)(1 << shift_no)) > 0;
        }

        private void ClearFlags()
        {
            //we don't have memset in c#, so we do this instead
            for(int i = 0; i < flag_arr.Length; ++i)
            {
                flag_arr[i] = 0;
            }
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

        //events to run, tied with button IDs
        public enum ButtonEventNumbers
        {
            Ad = 1000,
            Bd = 1001,
            Cd = 1002,
            Dd = 1003,
            Ed = 1004,
            Fd = 1005,
            Gd = 1006,
            Hd = 1007,
            Id = 1008,
            Jd = 1009,

            Au = 2000,
            Bu = 2001,
            Cu = 2002,
            Du = 2003,
            Eu = 2004,
            Fu = 2005,
            Gu = 2006,
            Hu = 2007,
            Iu = 2008,
            Ju = 2009,
        };

        public event IntDelegate? RunEventHandler; //sends output to TSC engine

        //can now parse multiple inputs at once
        public void ParseResponse(string input)
        {
            //multi-line test
            //input = "<1:1:1:1000\n<1:1:1:2000\n";

            try
            {
                //split by command delimiter
                string[] split_commands = Regex.Split(input, "(?=<)");

                foreach (var command in split_commands) {

                    //test
                    //input = "<1:13:1:12 O\n";

                    string[] split_strings = Regex.Split(command, "[<:]");

                    //all responses should have 4 parts (+1 for anything that comes before the '<')
                    if (split_strings.Length < 5)
                    {
                        continue;
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
                                //string[] split_response = Regex.Split(response_data, " ");
                                //int button_id = Int32.Parse(split_response[0]);
                                //bool status = split_response[1][0] == 'O';
                                ////run event based on button number + status "on" events are 1000 range, "off" events are 2000 range
                                //int event_num = 1000 * (status ? 1 : 2) + button_id;

                                //now we run the command prescribed directly by the response.
                                int event_num = Int32.Parse(response_data);
                                RunEventHandler?.Invoke(event_num);

                                break;
                            }
                    }

                }


            }
            catch
            {
                //do nothing if we get garbage data in (sometimes happens when we connect to a device)
            }
        }
    }

}
