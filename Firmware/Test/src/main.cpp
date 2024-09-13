
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


void loop()
{

    auto out_packet = RemoteGenericPacket(
        (const uint8_t*)packetstuff,
        sizeof(packetstuff),
        8
    );


    if(handler->SendPacket(out_packet, other_address))
        Serial.printf("sent packet: %s\n", out_packet.get_data_ptr());
    else
        Serial.printf("TX Fail!\n");


    if(handler->CheckForPacket() == RX_SUCCESS) {
        auto packet = handler->GetLastPacket();
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

