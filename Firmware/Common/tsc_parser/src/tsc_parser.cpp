#include <Arduino.h>
#include <queue>
#include <vector>

#include "tsc_parser.h"

using std::vector;
using std::queue;


vector<uint8_t> TscCommand::stringify_command() {
    //number of argchars + 4 for leading + 1 for null terminator
    auto bytecount = COMMAND_LEN(command_count) + 5;
    vector<uint8_t> vec(bytecount, 0);
    size_t idx = 0;

    //decompose type into explicit chars, '<CMD'
    vec[idx++] = '<';
    vec[idx++] = type & 0xFF;
    vec[idx++] = (type >> 8) & 0xFF;
    vec[idx++] = (type >> 16) & 0xFF;

    //re-compose command argument into a TSC string (including over/underflows)
    for(int i = 0; i < command_count; ++i) {

        //break number into 4 digit character
        vec[idx++] = (uint8_t)(tsc_args[i] / 1000 + '0');
        vec[idx++] = (uint8_t)((tsc_args[i] % 1000) / 100 + '0');
        vec[idx++] = (uint8_t)((tsc_args[i] % 100)  / 10 + '0');
        vec[idx++] = (uint8_t)((tsc_args[i] % 10)  / 1 + '0');

        //add argument delimiter
        if(i < command_count - 1) {
            vec[idx++] = ':';
        }
    }

    return vec;
}


TscParser::TscParser() {}

void TscParser::parse_tsc(const char* tsc_string) {

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
            case TSC_SLT: {
                //set light targets
                if(!parse_tsc_arg_count(&cursor, end_cursor, 3, TSC_SLT)) {
                    hit_end = true;
                    continue;
                }
                break;
            }
            case TSC_STS: {
                //set light transition speed
                if(!parse_tsc_arg_count(&cursor, end_cursor, 2, TSC_STS)) {
                    hit_end = true;
                    continue;
                }
                break;
            }
            case TSC_SLC: {
                //set light color
                if(!parse_tsc_arg_count(&cursor, end_cursor, 3, TSC_SLC)) {
                    hit_end = true;
                    continue;
                }
                break;
            }
            case TSC_SLR: {
                //
                if(!parse_tsc_arg_count(&cursor, end_cursor, 0, TSC_SLR)) {
                    hit_end = true;
                    continue;
                }
                break;
            }
            case TSC_SLS: {
                if(!parse_tsc_arg_count(&cursor, end_cursor, 6, TSC_SLS)) {
                    hit_end = true;
                    continue;
                }
                break;
            }
            case TSC_TGT: {
                if(!parse_tsc_arg_count(&cursor, end_cursor, 1, TSC_TGT)) {
                    hit_end = true;
                    continue;
                }
                break;
            }
            //unused commands (should be processed application-side): advance cursor to next command
            //case TSC_WAI:
            //case TSC_PSH:
            //case TSC_POP:
            //case TSC_EVE:
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

int TscParser::get_queue_length() {
    return command_queue.size();
}

bool TscParser::pop_tsc_command(TscCommand& result) {
    if(command_queue.size() > 0) {
        result = command_queue.front();
        command_queue.pop();
        return true;
    }
    return false;
}

bool TscParser::peek_tsc_command(TscCommand& result) {
    if(command_queue.size() > 0) {
        result = command_queue.front();
        return true;
    }
    return false;
}

int16_t TscParser::get_numeric(const char** cursor, bool should_increment) {
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

bool TscParser::parse_tsc_arg_count(const char** cursor, const char* end_cursor, int arg_count, TscCommandType type) {
    
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






















