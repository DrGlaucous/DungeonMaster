
#include <Arduino.h>

#include <radio_now_handler.h>
#include <configuration.h>
#include <tsc_parser.h>
#include <vector>

using std::vector;

const char key_net[] = ENCRYPTKEY_NETWORK;
const DeviceInfo& my_id = g_dongle_1;
const DeviceInfo* current_device_target = &g_dongle_1; //target ourselves by default

RadioNowHandler* handler;

TscParser parser;

//input serial data will be split at random points, so we need to organize it by newlines (there may be more than one command per string)
char* leftover_data = NULL;
int leftover_data_size = 0;

//if-else chain to select proper device with UUID. uses dongle UUID if not found
void set_radio_target(size_t uuid) {
    
    if(g_jcontroller_1.uu_id == uuid) {
        current_device_target = &g_jcontroller_1;
    } else if(g_box_1.uu_id == uuid) {
        current_device_target = &g_box_1;
    } else {
        current_device_target = &g_dongle_1;
    }
}

//run actions based on what TSC commands we get. This is different for each device type
void run_parse_actions() {
    //run actions based on gotten commands
    TscCommand command;
    while(parser.pop_tsc_command(command)) {

        switch(command.type) {
            case TSC_TGT: {
                set_radio_target(command.tsc_args[0]);
                //feedback that the command succeeded
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "TGT: Ok");
                Serial.printf("%s\n", response.get_data_ptr());

                break;
            }
            default: {
                //forward command to target device
                auto string_cmd = command.stringify_command();
                //RemoteGenericPacket packet = RemoteGenericPacket(&string_cmd[0], string_cmd.size(), PacketTscCommand);
                RemoteGenericPacket packet = RemoteGenericPacket((uint8_t*)&string_cmd[0], string_cmd.size(), PacketTscCommand);
                
                //if it was addressed to us, don't try to send it
                if((char*)current_device_target->mac_address == my_id.mac_address) {
                    //send an error directly back to the serial terminal
                    auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "Command not valid for this device!");
                    Serial.printf("%s\n", response.get_data_ptr());
                } else {

                    //Debug
                    //Serial.printf("Sent packet to %d\n", current_device_target->uu_id);
                    //Serial.printf("%s\n", &string_cmd[0]);

                    handler->send_packet(packet, (char*)current_device_target->mac_address);
                }
                break;
            }
        }
    }
}


void setup()
{
    Serial.begin(115200);

    handler = new RadioNowHandler(
        NETWORKID,
        key_net,
        my_id.mac_address
    );

    //set up network devices
    handler->add_peer(
        g_jcontroller_1.mac_address,
        g_jcontroller_1.local_master_key
    );
    handler->add_peer(
        g_box_1.mac_address,
        g_box_1.local_master_key
    );

    Serial.print("ready\n");
}


void loop() {



    /*
    auto byte_count = Serial.available();
    if(byte_count > 0) {

        char* byte_holder = (char*)calloc(byte_count + 1, 1);
        Serial.readBytes(byte_holder, byte_count);

        //Serial.printf("%s~~", byte_holder);

        parser.parse_tsc(byte_holder);
        
        free(byte_holder);

        run_parse_actions();
    }
    */


    
    //check for commands sent over serial
    auto byte_count = Serial.available();
    if(byte_count > 0) {

        //put new serial data into vector
        char* byte_holder = (char*)calloc(byte_count + 1, 1);
        Serial.readBytes(byte_holder, byte_count);

        //find index of last line break
        char* last_one = byte_holder + byte_count;
        for(; last_one >= byte_holder; --last_one) {
            if(*last_one == '\n') {break;}
        }

        int leftover_section_size = (int)(byte_holder + byte_count - last_one);
        int initial_section_size = (int)(last_one - byte_holder);


        //make new array for first section and process it immediately
        //and store any leftovers in the leftover array
        if(initial_section_size > 0) {


            char* full_holder = (char*)calloc(initial_section_size + leftover_data_size + 1, 1);
            memcpy(full_holder, leftover_data, leftover_data_size);
            memcpy(full_holder + leftover_data_size, byte_holder, initial_section_size);

            //run commands on this array
            parser.parse_tsc(full_holder);

            //Serial.printf("~~~~~~~~~~~~~~~~%s\n~~~~~~~~~~~~~~", full_holder);

            //run actions based on commands
            run_parse_actions();
    
            free(full_holder);


            //store leftovers in new leftover array
            if(leftover_section_size) {
                if(leftover_data != NULL) {
                    free(leftover_data);
                }
                leftover_data = (char*)calloc(leftover_section_size + 1, 1);
                memcpy(leftover_data, byte_holder + initial_section_size, leftover_section_size);
                leftover_data_size = leftover_section_size;

            }


        }
        else {
            //copy all data to the leftover array for later

            char* full_holder = (char*)calloc(initial_section_size + leftover_data_size + 1, 1);
            memcpy(full_holder, leftover_data, leftover_data_size);
            memcpy(full_holder + leftover_data_size, byte_holder + initial_section_size, leftover_section_size);

            if(leftover_data != NULL) {
                free(leftover_data);
            }
            leftover_data_size += leftover_section_size;
            leftover_data = full_holder;

        }


        free(byte_holder);
        

    }
    


    /*
    if(byte_count > 0) {

        //+1 extra for null terminator, init to 0 + byte count from last time
        char* byte_holder = (char*)calloc(byte_count + leftover_bytes_count + 1, 1);

        //copy over leftovers
        if(leftover_bytes_count > 0 && leftover_bytes != NULL) {
            memcpy(byte_holder, leftover_bytes, leftover_bytes_count);
        }
        char* new_starting_read = byte_holder + leftover_bytes_count;
        Serial.readBytes(new_starting_read, byte_count);

        //parse the commands
        auto last_point = parser.parse_tsc(byte_holder);

        //number of non-null chars left unparsed by the TSC parser, which will usually appear if commands are split between serial transmissions
        leftover_bytes_count = (byte_count + leftover_bytes_count) - (int)(last_point - byte_holder);


        //Debug
        Serial.printf("GOT: %s\n", byte_holder);
        //Serial.printf("Parsed: %d\n\n------\n", parser.get_queue_length() );
        //Serial.printf("Left over: %d, %d, %s\n", (int)(last_point - byte_holder), byte_count, last_point);


        //response print if what was sent yielded 0 valid commands:
        if(parser.get_queue_length() == 0) {

            auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "Warning: No commands were successfully compiled\n");
            Serial.printf("%s\n%s\n", response.get_data_ptr(), byte_holder);
            Serial.printf("LB: %s\n", leftover_bytes);
        }


        //store leftovers for next round
        if(leftover_bytes_count > 0) {
            if(leftover_bytes != NULL) {
                free(leftover_bytes);
            }
            //re-allocate an array to store the leftovers (+1 for NT)
            leftover_bytes = (char*)calloc(leftover_bytes_count + 1, 1);
            memcpy(leftover_bytes, last_point, leftover_bytes_count);
            //Serial.printf("LB: %s\n", leftover_bytes);
        }


        free(byte_holder);

        //run actions based on commands
        run_parse_actions();


    }
    */



    //check for packets gotten back from perepherials
    if(handler->check_for_packet() == RX_SUCCESS) {

        auto packet = handler->get_last_packet();

        //Debug
        //Serial.printf("GOT RADIO PACKET\n");
        //Serial.printf("%s\n", packet.get_data_ptr());
        
        switch(packet.get_packet_type()) {
            //forward message to program (note: assumes packet ends in null terminator!)
            case PacketResponse: {
                Serial.printf("%s\n", packet.get_data_ptr());
                break;
            }
            //tsc command, run TSC actions
            default: {
                parser.parse_tsc((const char*)packet.get_data_ptr());
                run_parse_actions();
                break;
            }
        }

    }


}










