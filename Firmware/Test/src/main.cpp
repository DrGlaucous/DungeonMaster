
#include <Arduino.h>

#include <radio_now_handler.h>
#include <configuration.h>

#include "tsc_parser.h"

#define SENDER_1

RadioNowHandler* handler;

#define DONGLE_ADDRESS {0x30, 0xAE, 0xA4, 0x07, 0x0D, 0x64}
#define BOX_ADDRESS {0xA0, 0x0E, 0x04, 0x0F, 0xFD, 0x64}

#ifdef SENDER_1
char my_address[] = DONGLE_ADDRESS;
char other_address[] = BOX_ADDRESS;
char key_net[] = ENCRYPTKEY_NETWORK;
char key_peer[] = ENCRYPTKEY_PEER;
char packetstuff[] = "OUTPUT SENDER\0";
#else
char my_address[] = BOX_ADDRESS;
char other_address[] = DONGLE_ADDRESS;
char key_net[] = ENCRYPTKEY_NETWORK;
char key_peer[] = ENCRYPTKEY_PEER;
char packetstuff[] = "OUTPUT GETTER\0";
#endif


TscParser parser;

void setup()
{
    Serial.begin(115200);


    handler = new RadioNowHandler(
        NETWORKID,
        key_net,
        my_address
    );

    handler->add_peer(
        other_address,
        key_peer
    );

    Serial.print("ready\n");
}


void loop() {

    auto byte_count = Serial.available();
    if(byte_count > 0) {
        //+1 extra for null terminator, init to 0
        char* byte_holder = (char*)calloc(byte_count + 1, 1);
        Serial.readBytes(byte_holder, byte_count);

        //parse out the commands
        parser.parse_tsc(byte_holder);

        free(byte_holder);

        int command_count = parser.get_queue_length();

        if(!command_count)
            Serial.printf("Did not get any valid commands!\n");

        for(int i = 0; i < command_count; ++i) {
            TscCommand command;
            auto result = parser.pop_tsc_command(command);

            Serial.printf("Command no: %d, argc: %d, type: %d\n", i, command.command_count, command.type);

        }


    }


}




void loop2()
{

    auto out_packet = RemoteGenericPacket(
        (const uint8_t*)packetstuff,
        sizeof(packetstuff),
        8
    );


    if(handler->send_packet(out_packet, other_address))
        Serial.printf("sent packet: %s\n", out_packet.get_data_ptr());
    else
        Serial.printf("TX Fail!\n");


    if(handler->check_for_packet() == RX_SUCCESS) {
        auto packet = handler->get_last_packet();
        auto datalen = packet.get_data_len();
        if(packet.get_data_len() > 0) {

            auto pp = packet.get_data_ptr();
            Serial.printf("Got packet: %d, %s\n", datalen, pp);
        }
        else
            Serial.printf("Error: Got packet of 0 length!\n");
    }

    delay(1000);


}

