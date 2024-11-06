
#include <Arduino.h>

#include <Bounce2.h> 
#include <radio_now_handler.h>
#include <configuration.h>
#include <tsc_parser.h>
#include <light_effect_handler.h>


const char key_net[] = ENCRYPTKEY_NETWORK;
const DeviceInfo& my_id = g_box_1;

RadioNowHandler* handler;


TscParser parser;

//2 buttons for the remote
int array_size = 2;
int input_array[] = {19, 32};

Bounce2::Button button_array[2] = {};

//events to press and release with each button
int press_array[] = {
    BlueReadyActive,
    RedReadyActive,
    ArenaDoorActive,
};
int release_array[] = {
    BlueReadyInactive,
    RedReadyInactive,
    ArenaDoorInactive,
};


//LightEffectHandler le_handler = LightEffectHandler(0, (const DirectLedPins*)remote_led_addresses, 8);

//test for led strip
LightEffectHandler* le_handler;// = LightEffectHandler(8, NULL, 0);

unsigned long last_time_millis = 0;


//run actions based on what TSC commands we get. This is different for each device type
void run_parse_actions() {
    //run actions based on gotten commands
    TscCommand command;
    while(parser.pop_tsc_command(command)) {

        switch(command.type) {
            case TSC_SLT:
            case TSC_PLC:
            case TSC_CLC:           
            case TSC_RLI:
            case TSC_RLT:
            case TSC_SLR: {

                le_handler->parse_command(command);

                //feedback that the command succeeded
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "GOT Command: Ok");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);
                break;
            }

            //todo: other remote-commands
            default: {
                //error: we should not be getting this command!
                auto response = assemble_response_packet(my_id.type, my_id.uu_id, PacketGetOk, "Command not valid for this device!");
                handler->send_packet(response, (char*)g_dongle_1.mac_address);

                break;
            }
        }
    }
}


void setup()
{

    Serial.begin(115200);


    //config for box LEDs
    le_handler = new LightEffectHandler(BOX_RGB_LED_COUNT, (const DirectLedPins*)box_led_addresses, sizeof(box_led_addresses) / sizeof(DirectLedPins));


    handler = new RadioNowHandler(
        NETWORKID,
        key_net,
        my_id.mac_address,
        20
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
        //pinMode(input_array[i], INPUT_PULLUP);
        //pinMode(output_array[i], OUTPUT);

        button_array[i].attach(input_array[i], INPUT_PULLUP);
        button_array[i].interval(20);
        button_array[i].setPressedState(0);
    }


    last_time_millis = millis();

    Serial.println("ready");

}

void loop() {

    //check for packets gotten from dongle
    while(handler->check_for_packet() == RX_SUCCESS) {

        auto packet = handler->get_last_packet();


        //Serial.printf("GOT RADIO PACKET\n");
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


        button_array[i].update();

        if(button_array[i].changed()) {

            int event_num;
            if(button_array[i].pressed()) {
                event_num = press_array[i];
            } else {
                event_num = release_array[i];
            }

            auto better_packet = assemble_response_packet(my_id.type, my_id.uu_id, ResponseType::ButtonStatus, std::to_string(event_num).c_str());
            TXStatus status = handler->send_packet(better_packet, g_dongle_1.mac_address);
            Serial.printf("%s\n", (char*)better_packet.get_data_ptr());
        }


    }

    auto time_millis = millis();

    auto delta = time_millis - last_time_millis;

    //only do this a minimum of 10 ms because setting analogWrite too fast results in nothing actually being written
    if(delta > 10) {
        le_handler->tick(delta);
        le_handler->set_out();
        last_time_millis = time_millis;
    }

}




