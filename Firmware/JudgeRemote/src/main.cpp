
#include <Arduino.h>

#include <Bounce2.h> 
#include <radio_now_handler.h>
#include <configuration.h>
#include <tsc_parser.h>
#include <light_effect_handler.h>


const char key_net[] = ENCRYPTKEY_NETWORK;
const DeviceInfo& my_id = g_jcontroller_1;

RadioNowHandler* handler;


TscParser parser;

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
//int output_array[] = {4, 25, 16, 26, 17, 12, 5, 13};
//int last_button_state_array[] = {0,0,0,0,0,0,0,0};

Bounce2::Button button_array[8] = {};

//events to press and release with each button
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
    //le_handler = new LightEffectHandler(BOX_RGB_LED_COUNT, (const DirectLedPins*)box_led_addresses, sizeof(box_led_addresses) / sizeof(DirectLedPins));

    //config for remote LEDs
    le_handler = new LightEffectHandler(REMOTE_RGB_LED_COUNT, (const DirectLedPins*)remote_led_addresses, sizeof(remote_led_addresses) / sizeof(DirectLedPins));


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


    //setup analog reader and low battery LED
    //pinMode(LED_BUILTIN, OUTPUT);
    pinMode(34, INPUT);


    last_time_millis = millis();

    Serial.println("ready");

}

void loop() {

    //low battery error: stop running normal stuff and instead do this
    //our safe target voltage should be 9.6V, or 3.2v per cell
    //volt to analog reading tests:
    //17.1 - 1715 || 8.8 - 790 || 9.6 - 880
    //check for voltage level greater than 0 in case we're running tests plugged into USB power
    auto voltage_level = analogRead(34);
    if (voltage_level > 0 && voltage_level < 880) {

        //blink rapidly all button lights
        while(1) {
            for(int i = 0; i < sizeof(remote_led_addresses) / sizeof(DirectLedPins); ++i) {
                auto pin_no = remote_led_addresses[i].r_pin;
                analogWrite(pin_no, 255);
            }
            delay(100);
            for(int i = 0; i < sizeof(remote_led_addresses) / sizeof(DirectLedPins); ++i) {
                auto pin_no = remote_led_addresses[i].r_pin;
                analogWrite(pin_no, 0);
            }
            delay(100);
            Serial.printf("Low Power! Please replace battery with a charged 12v\n");
        }
    }


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




