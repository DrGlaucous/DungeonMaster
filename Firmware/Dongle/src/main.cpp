
#include <Arduino.h>

#include <radio_now_handler.h>
#include <configuration.h>
#include <tsc_parser.h>

const char key_net[] = ENCRYPTKEY_NETWORK;
const DeviceInfo& my_id = g_dongle_1;
const DeviceInfo* current_device_target = &g_dongle_1; //target ourselves by default

RadioNowHandler* handler;


TscParser parser;


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
                    auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "Invalid command!");
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

    //check for commands sent over serial
    auto byte_count = Serial.available();
    if(byte_count > 0) {
        //+1 extra for null terminator, init to 0
        char* byte_holder = (char*)calloc(byte_count + 1, 1);
        Serial.readBytes(byte_holder, byte_count);

        //parse the commands
        parser.parse_tsc(byte_holder);

        //Debug
        //Serial.printf("GOT: %s\n", byte_holder);
        //Serial.printf("Parsed: %d\n", parser.get_queue_length() );


        free(byte_holder);

        //run actions based on commands
        run_parse_actions();


    }

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










