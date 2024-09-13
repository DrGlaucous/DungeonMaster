#pragma once

#include <Arduino.h>
#include <queue>

using std::queue;

#define TSC_MAX_ARG_COUNT 6

//turns a command into a number that can be matched in a switch statement
#define IS_COMMAND(c1, c2, c3) (c1 | (c2 << 8) | c3 << 16)

#define COMMAND_LEN(argc) (argc - (1 * !!argc) + (argc * 4))

enum TscCommandType {
    TSC_NULL, //nul
    TSC_TGT, //nul
    TSC_SLT,
    TSC_STS,
    TSC_SLC,
    TSC_SLR,
    TSC_SLS,
    TSC_WAI, //nul
    TSC_PSH, //nul
    TSC_POP, //nul
};

//no need for string commands: those 
struct TscCommand {
    TscCommandType type = TSC_NULL;
    int16_t tsc_args[TSC_MAX_ARG_COUNT] = {};
    uint8_t command_count = 0;
};

class TscParser {

    public:

    TscParser() {};


    //take c string refrence and fill the command queue with processed commands
    //note: string MUST be null-terminated
    void parse_tsc(const char* tsc_string) {

        if(tsc_string == NULL) {
            return;
        }

        auto cursor = tsc_string;

        //find null terminator
        while(*cursor != '\0') {
            ++cursor;
        }
        auto end_cursor = cursor; //cache position of end-of-string null
        cursor = tsc_string;

        bool hit_end = (*tsc_string == '\0');

        //run until command is empty
        while(!hit_end) {

            //check for null terminator mid-command
            if(cursor + 4 > end_cursor) {
                hit_end = true;
                continue;
            }

            //increment pointer at the same time we're matching
            char a = *++cursor;
            char b = *++cursor;
            char c = *++cursor;
            ++cursor;
            switch(IS_COMMAND(a, b, c)) {
                case IS_COMMAND('S', 'L', 'T'): {
                    //set light targets
                    if(!parse_tsc_arg_count(&cursor, end_cursor, 3, TSC_SLT)) {
                        hit_end = true;
                        continue;
                    }
                    break;
                }
                case IS_COMMAND('S', 'T', 'S'): {
                    //set light transition speed
                    if(!parse_tsc_arg_count(&cursor, end_cursor, 2, TSC_STS)) {
                        hit_end = true;
                        continue;
                    }
                    break;
                }
                case IS_COMMAND('S', 'L', 'C'): {
                    //set light color
                    if(!parse_tsc_arg_count(&cursor, end_cursor, 3, TSC_SLC)) {
                        hit_end = true;
                        continue;
                    }
                    break;
                }
                case IS_COMMAND('S', 'L', 'R'): {
                    //
                    if(!parse_tsc_arg_count(&cursor, end_cursor, 0, TSC_SLR)) {
                        hit_end = true;
                        continue;
                    }
                    break;
                }
                case IS_COMMAND('S', 'L', 'S'): {
                    if(!parse_tsc_arg_count(&cursor, end_cursor, 6, TSC_SLS)) {
                        hit_end = true;
                        continue;
                    }
                    break;
                }
                //unused commands: advance cursor to next command
                //case IS_COMMAND('T', 'G', 'T'):
                //case IS_COMMAND('W', 'A', 'I'):
                //case IS_COMMAND('P', 'S', 'H'):
                //case IS_COMMAND('P', 'O', 'P'):
                default: {
                    break;
                }


            }

            //iterate until we reach the next command or the end of the string buffer
            while(*cursor != '<') {
                //halt parsing if we hit a null terminator
                if(*cursor == '\0') {
                    hit_end = true;
                    break;    
                }
                ++cursor;
            }

        }


    }

    inline int get_queue_length() {
        return command_queue.size();
    }

    TscCommand pop_tsc_command() {
        TscCommand front = command_queue.front();
        command_queue.pop();
        return front;
    }

    TscCommand peek_tsc_command() {
        TscCommand front = command_queue.front();
        return front;
    }

    private:

    queue<TscCommand> command_queue;

    //gets a number and optionally increments the cursor
    int16_t get_numeric(const char** cursor, bool should_increment = true) {
        const char* cursor2 = *cursor;
        int16_t b = 0;
        b += (*cursor2++ - '0') * 1000;
        b += (*cursor2++ - '0') * 100;
        b += (*cursor2++ - '0') * 10;
        b += *cursor2++ - '0';

        if(should_increment)
            *cursor = cursor2;

        return b;
    }

    //returns true if parse was successful, pushes command arguments to the command queue
    bool parse_tsc_arg_count(const char** cursor, const char* end_cursor, int arg_count, TscCommandType type) {
        
        //clamp within range
        if(arg_count > TSC_MAX_ARG_COUNT) {
            arg_count = TSC_MAX_ARG_COUNT;
        }

        //check for valid cursor count
        if(*cursor + COMMAND_LEN(arg_count) > end_cursor) {
            return false;
        }

        TscCommand command = {};
        command.type = type;

        //parse all arguments
        for(int i = 0; i < arg_count; ++i) {
            int x = get_numeric(cursor);
            *cursor += 1 * (i != arg_count - 1); //only increment if we're NOT on the last argument in the list

            command.tsc_args[i] = x;
            command.command_count += 1;
        }
        //add to command stack
        command_queue.push(command);

        return true;
    }


};





