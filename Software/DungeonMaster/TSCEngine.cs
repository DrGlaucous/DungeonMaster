using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace DungeonMaster
{

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

    internal class TSCEngine
    {

        //constructor
        public TSCEngine() { }


        enum EngineState {
            Idle,
            Running,
            Waiting,
        }


        private EngineState state = EngineState.Idle;
        private List<TSCEvent> event_list = [];
        private bool key_state = false;

        //what events to run next 
        private Queue<int> event_queue = new();

        //used with <PSH and <POP to return to previous event + location
        private Stack<(int event_no, int cmd_no)> event_stack = new();

        //converts a string TSC script into an array of event objects internally, erases old event list!
        public void LoadAndParseScript(string script) {


            string test_string = "#3533OKvanilla\n\n<CMD<CM2random_text<APP0000:1111<GRN2222:3333\n#3563nicevanilla\ngood_practice";

            script = test_string;

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
            foreach (var istring in split_strings) {

                if (istring == String.Empty) {
                    continue;
                }

                //trim off extra stuff after event number, so something like #1234abcd\n will be #1234\n
                string event_trimmed = Regex.Replace(istring, "(?<=#....).*", "");

                //get event number
                int event_num = GetNumberFromString(event_trimmed[1..]);

                TSCEvent tsc_event = new();
                tsc_event.Event = event_num;

                //split by opcode
                string[] command_array = Regex.Split(istring, "(?=<...)");
                foreach (var command in command_array) {
                    //first entry, will either be empty string or contain non-command lead-ins
                    if (command[0] != '<') {
                        continue;
                    }

                    TSCObject obj = new();

                    //get opcode
                    obj.OPCode = command[1..4]; //note: range operator 1..4 is [1,4)

                    //get list of raw args (determining how to enterperet each one will happen when we actually run the script)
                    if (command.Length > 4) {

                        //break by delimiter
                        string[] argument_array = Regex.Split(command[4..], ":");
                        foreach (var arg in argument_array) {
                            //first entry should not be discarded here
                            obj.arguments.Add(arg);
                        }
                    }

                    tsc_event.Commands.Add(obj);

                }
                
                event_list.Add(tsc_event);
            }
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


        //drops whatever its doing and begins running this next event. if the event doesn't exist, the state is set to idle
        public void RunEvent(int event_no, int command_no) { 
            state = EngineState.Running; //may not actually need to set this since this runs in one cycle.

            for (int i = 0; i < event_list.Count; ++i) {
                //event matches, run it
                if (event_list[i].Event == event_no) {

                    //for each sequential command in the command list
                    for (int j = 0; j < event_list[i].Commands.Count; ++j) {
                        var command = event_list[i].Commands[j]; //for ease-of-refrence
                        switch (command.OPCode) {
                            case "TGT":
                            case "SLT":
                            case "STS":
                            case "SLC":
                            case "SLR":
                            case "SLS":
                            default:
                                {
                                    //send these commands out to the connected device
                                    string output = command.ReassembleCmd();
                                    break;
                                }
                            case "WAI":
                                {
                                    break;
                                }
                            case "PSH":
                                {
                                    //jump to next event if we have a valid event argument
                                    try
                                    {
                                        var event_num = command.arguments[0];
                                        event_stack.Push((i, j)); //keep track of return location
                                        RunEvent(event_no, 0);
                                    }
                                    catch { }
                                    break;
                                }
                            case "POP":
                                {
                                    break;
                                }
                            case "KEY":
                                {
                                    key_state = true;
                                    break;
                                }
                            case "FRE":
                                {
                                    key_state = false;
                                    break;
                                }
                            case "EVE":
                                {
                                    //jump to next event if we have a valid event argument
                                    try
                                    {
                                        var event_num = command.arguments[0];
                                        RunEvent(event_no, 0);
                                        return; //don't run any events after this one since we've technically handed off the events to "EVE"
                                    }
                                    catch { }

                                    break;
                                }
                        }
                    }


                    break; //found the event, break out
                }

                
            }

            //return to idle state
            state = EngineState.Idle;
        }

        //push event to the event queue to be run when the earlier events finish
        public void PushEvent(int event_no) {
            event_queue.Enqueue(event_no);

            //if not keyed or engine is idleing, begin the event immediately
            if(!key_state || state == EngineState.Idle) {
                PopEvent();
            }
            
        }

        //checks for an event in the queue and runs it, if queue is empty, it sets the TSC state to idle
        private void PopEvent() {
            if (event_queue.Count() > 0)
            {
                var next_event = event_queue.Dequeue();
                RunEvent(next_event, 0);
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

    }
}
