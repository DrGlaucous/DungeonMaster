using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster
{

    internal class TSCObject {
        public string OPCode = String.Empty;
        public List<int> arguments = [];
        public List<string> str_arguments = [];  
    }
    internal class TSCEvent {
        public int Event = 0;
        public List<TSCObject> commands = []; 
    }

    internal class TSCParse
    {

        //constructor
        public TSCParse() { }


        enum EngineState {
            Idle,
            Running,
            Waiting,
        }

        private EngineState state = EngineState.Idle;
        private List<TSCEvent> event_list = [];

        //internal variables
        //private Vector<char> script_buffer;
        //private string script_buffer = String.Empty;
        //private int cursor = 0;


        enum CommentState { 
            Normal,
            FoundFslash, //finds one of 2 needed '/'s to enter comment mode
            CommentMode,

        }

        //converts a string TSC script into an array of event objects
        public void LoadAndParseScript(string script) {

            CommentState c_state = CommentState.Normal;

            for (int i = 0; i < script.Length; ++i) {

                //check for comments
                if (script[i] == '/') {
                    switch (c_state) {
                        case CommentState.FoundFslash: {
                                c_state = CommentState.CommentMode;
                                break;
                            }
                        default: {
                                c_state = CommentState.FoundFslash;
                                break;
                            }
                    }
                    
                }

                //by this point, script[i] should only have non-comments in it
                if (c_state == CommentState.CommentMode) {
                
                }


            }
            
        }






    }
}
