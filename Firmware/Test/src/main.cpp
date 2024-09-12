
#include <Arduino.h>

#include <radio_now_handler.h>
#include "configuration.h"


RadioNowHandler* handler;

#ifdef SENDER_1
char my_address[] = DONGLE_ADDRESS;
char other_address[] = BOX_ADDRESS;
char key_net[] = ENCRYPTKEY_NETWORK;
char key_peer[] = ENCRYPTKEY_PEER;
char packetstuff[] = "OUTPUT SENDER\0";
#elif
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
        if(packet.get_data_len() > 0)
            Serial.printf("Got packet: %s\n", packet.get_data_ptr());
        else
            Serial.printf("Error: Got packet of 0 length!\n");
    }

    delay(1000);


}





