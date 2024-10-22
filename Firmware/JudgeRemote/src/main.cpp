
#include <Arduino.h>

#include <radio_now_handler.h>
#include <configuration.h>
#include <tsc_parser.h>

#include "light_effect_handler.h"
#include <FastLED.h>

const char key_net[] = ENCRYPTKEY_NETWORK;
const DeviceInfo& my_id = g_jcontroller_1;

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
            //todo: other remote-commands


            default: {
                //error: we should not be getting this command!
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "Invalid command!");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);

                break;
            }
        }
    }
}

/*

button layout:
A|B
C|D
E|F
G|H

input pins
19|32
21|33
22|27
23|14

output pins
4|25
16|26
17|12
5|13

*/

int array_size = 8;
int input_array[] = {19, 32, 21, 33, 22, 27, 23, 14};
int output_array[] = {4, 25, 16, 26, 17, 12, 5, 13};
int last_button_state_array[] = {0,0,0,0,0,0,0,0};
int press_array[] = {
    ButtonAActive,
    ButtonBActive,
    ButtonCActive,
    ButtonDActive,
    ButtonEActive,
    ButtonFActive,
    ButtonGActive,
    ButtonHActive,
};
int release_array[] = {
    ButtonAInactive,
    ButtonBInactive,
    ButtonCInactive,
    ButtonDInactive,
    ButtonEInactive,
    ButtonFInactive,
    ButtonGInactive,
    ButtonHInactive,
};


LightEffectHandler le_handler = LightEffectHandler(0, (DirectLedPins*)remote_led_addresses, 8);

void setup()
{

    Serial.begin(115200);

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


    //voltmeter
    pinMode(34, INPUT);


    //test
    //buttons + leds
    for(int i = 0; i < array_size; ++i) {
        pinMode(input_array[i], INPUT_PULLUP);
        //pinMode(output_array[i], OUTPUT);
    }


    Serial.println("ready");

}

void loop() {


    //check for packets gotten from dongle
    if(handler->check_for_packet() == RX_SUCCESS) {

        auto packet = handler->get_last_packet();


        Serial.printf("GOT RADIO PACKET\n");
        Serial.printf("%s\n", packet.get_data_ptr());

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


    for(int i = 0; i < array_size; ++i) {
        int button_state = digitalRead(input_array[i]);

        //only do things on state change
        if(last_button_state_array[i] != button_state) {
            //low = on
            if(!button_state) {
                //digitalWrite(output_array[i], 1);

                string response = "<" + (int)my_id.type + ':' + (int)my_id.uu_id + ':' + (int)ResponseType::ButtonStatus + ':' + press_array[i] + '\n';
                auto packet = RemoteGenericPacket((const uint8_t*)response.c_str(), response.size(), (size_t)ResponseType::ButtonStatus);

                TXStatus status = handler->send_packet(packet, g_dongle_1.mac_address);

                Serial.printf("%d\n", status);


            } else {
               // digitalWrite(output_array[i], 0);
            }
        }
        last_button_state_array[i] = button_state;
        



    }


}




