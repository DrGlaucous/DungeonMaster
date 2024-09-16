
#include <Arduino.h>

#include <radio_now_handler.h>
#include <configuration.h>
#include <tsc_parser.h>

const char key_net[] = ENCRYPTKEY_NETWORK;
const DeviceInfo& my_id = g_box_1;

RadioNowHandler* handler;


TscParser parser;


//run actions based on what TSC commands we get. This is different for each device type
void run_parse_actions() {
    //run actions based on gotten commands
    TscCommand command;
    while(parser.pop_tsc_command(command)) {

        switch(command.type) {
            case TSC_SLT: {
                //feedback that the command succeeded
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "SLT: Ok");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);
                break;
            }
            case TSC_STS: {
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "STS: Ok");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);
                break;
            }
            case TSC_SLC: {
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "SLC: Ok");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);
                break;
            }
            case TSC_SLR: {
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "SLR: Ok");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);
                break;
            }
            case TSC_SLS: {
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "SLS: Ok");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);
                break;
            }
            default: {
                //error: we should not be getting this command!
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "Invalid command!");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);

                break;
            }
        }
    }
}


void setup()
{

    //Serial.begin(115200);

    handler = new RadioNowHandler(
        NETWORKID,
        key_net,
        my_id.mac_address
    );

    //set up network devices (only need the dongle since we're not talking to the other stuff directly)
    handler->add_peer(
        g_dongle_1.mac_address,
        g_dongle_1.local_master_key
    );

    //Serial.println("ready");

}


void loop() {


    //check for packets gotten from dongle
    if(handler->check_for_packet() == RX_SUCCESS) {

        auto packet = handler->get_last_packet();


        //Serial.printf("GOT RADIO PACKET\n");
        //Serial.printf("%s\n", packet.get_data_ptr());

        switch(packet.get_packet_type()) {
            //tsc command, run TSC actions
            case PacketTscCommand: {
                parser.parse_tsc((const char*)packet.get_data_ptr());
                run_parse_actions();
            }
            default: {
                break;
            }
        }

    }


}










