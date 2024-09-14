#pragma once

#include <Arduino.h>
#include <queue>
#include <vector>

using std::vector;
using std::queue;

#define TSC_MAX_ARG_COUNT 6

//turns a command into a number that can be matched in a switch statement
#define IS_COMMAND(c1, c2, c3) (c1 | (c2 << 8) | c3 << 16)
//converts argc into bytecount, so 2 args like xxxx:yyyy is 9 bytes
#define COMMAND_LEN(argc) (argc - (1 * !!argc) + (argc * 4))


enum TscCommandType {
    TSC_NULL = IS_COMMAND('\0', '\0', '\0'), //nul
    TSC_TGT = IS_COMMAND('T', 'G', 'T'), //dongle only
    TSC_SLT = IS_COMMAND('S', 'L', 'T'),
    TSC_STS = IS_COMMAND('S', 'T', 'S'),
    TSC_SLC = IS_COMMAND('S', 'L', 'C'),
    TSC_SLR = IS_COMMAND('S', 'L', 'R'),
    TSC_SLS = IS_COMMAND('S', 'L', 'S'),
    TSC_WAI = IS_COMMAND('W', 'A', 'I'), //pc only
    TSC_PSH = IS_COMMAND('P', 'S', 'H'), //pc only
    TSC_POP = IS_COMMAND('P', 'O', 'P'), //pc only
};

//no need for string commands: those 
class TscCommand {
    public:
    TscCommandType type = TSC_NULL;
    int16_t tsc_args[TSC_MAX_ARG_COUNT] = {};
    uint8_t command_count = 0;

    vector<uint8_t> stringify_command();

};

class TscParser {

    public:

    TscParser();


    //take c string refrence and fill the command queue with processed commands
    //note: string MUST be null-terminated, this does not process comments!
    void parse_tsc(const char* tsc_string);

    int get_queue_length();

    bool pop_tsc_command(TscCommand& result);

    bool peek_tsc_command(TscCommand& result);

    private:

    queue<TscCommand> command_queue;

    //gets a number and optionally increments the cursor
    int16_t get_numeric(const char** cursor, bool should_increment = true);

    //returns true if parse was successful, pushes command arguments to the command queue
    bool parse_tsc_arg_count(const char** cursor, const char* end_cursor, int arg_count, TscCommandType type);

};





