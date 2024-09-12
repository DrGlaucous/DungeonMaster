#include <Arduino.h>

#include <esp_now.h>
#include <esp_wifi.h>
#include <Wifi.h>

#include "radio_now_handler.h"


QueueHandle_t gotten_data_holder;
//do not write to this variable
unsigned long rec_time = {};
//unsigned long last_rec_time = {};

//called after packet was finished sending (we don't do anything in here for now)
void packet_sent_callback(const uint8_t *mac_addr, esp_now_send_status_t status)
{
    //Serial.print("\r\nLast Packet Send Status:\t");
    //Serial.println(status == ESP_NOW_SEND_SUCCESS ? "Delivery Success" : "Delivery Fail");
}

//called when a packet is gotten
void packet_got_callback(const uint8_t * mac, const uint8_t *incomingData, int len)
{
    //Serial.println("Got");
    //Serial.printf("Bytes received: %d, Num: %d\n", len, (int)*incomingData);
    //last_rec_time = rec_time;
    rec_time = millis();
    BaseType_t high_task_wakeup = pdFALSE;


    //holds the int + gotten data
    uint8_t len_concat[ESP_NOW_MAX_DATA_LEN + sizeof(int)] = {};
    memcpy(len_concat, &len, sizeof(len));
    memcpy(len_concat + sizeof(len), incomingData, len);

    xQueueOverwriteFromISR(gotten_data_holder, len_concat, &high_task_wakeup);
}

//delta time
unsigned long delta_time(unsigned long now, unsigned long last)
{
    //detect overflows
    if(now < last)
        return now + (UINT64_MAX - last);
    else
        return now - last;

}

//can only have one of these: this is shared for everyone
bool RadioNowHandler::some_initialized = false;

RadioNowHandler::RadioNowHandler(
    uint32_t net_id,
    const char* key_network,
    const char* my_address
)
{
    //do not make it more than once
    if(some_initialized) {
        return;
    }

    this->net_id = net_id;

    //initialize the queue if it is not already active
    if(gotten_data_holder == NULL)
    {
        //create the callback data queue if it's not already present (max esp_now queue size + size of int for storing packet size)
        gotten_data_holder = xQueueCreate(1, ESP_NOW_MAX_DATA_LEN + sizeof(int));
    }

    //check again for successful queue creation
    if(gotten_data_holder == NULL)
        return;


    //setup the radios (in "station" mode)
    WiFi.mode(WIFI_STA);

    //Init ESP-NOW
    if (esp_now_init() != ESP_OK) {
        Serial.println("Error initializing ESP-NOW");

        //destruct queue
        vQueueDelete(gotten_data_holder);
        return;
    }

    //add callback for sending packets
    if(esp_now_register_send_cb(packet_sent_callback) != ESP_OK) {
        Serial.println("Error initializing send callback");

        //destruct items
        vQueueDelete(gotten_data_holder);
        esp_now_deinit();

        return;
    }

    //add callback for getting packets
    if(esp_now_register_recv_cb(packet_got_callback) != ESP_OK) {

        //destruct items
        vQueueDelete(gotten_data_holder);
        esp_now_deinit();
        esp_now_unregister_send_cb();

        return;
    }


    //all important initializations have been completed, we can set these now.
    some_initialized = true;
    active = true;


    //encrypt
    if(key_network != NULL) {
        encrypted = true;
        esp_now_set_pmk((uint8_t*)key_network);
    }


    esp_wifi_set_mac(WIFI_IF_STA, (uint8_t*)my_address);

    auto set_channel_error = esp_wifi_set_channel(net_id, WIFI_SECOND_CHAN_ABOVE);
#ifdef DEBUG
    switch(set_channel_error)
    {
        default:
        case 0:
            Serial.println("Success!");
            break;
        case ESP_ERR_WIFI_NOT_INIT:
            Serial.println("ESP_ERR_WIFI_NOT_INIT");
            break;

        case ESP_ERR_WIFI_IF:
            Serial.println("ESP_ERR_WIFI_IF");
            break;
        case ESP_ERR_INVALID_ARG:
            Serial.println("ESP_ERR_INVALID_ARG");
            break;

    }

    
    Serial.print("[NEW] ESP32 Board MAC Address:  ");
    Serial.println(WiFi.macAddress());
#endif

}
RadioNowHandler::~RadioNowHandler()
{
    //we didn't construct anything: don't try to delete anything
    if(!active)
        return;

    //delete and derefrence queue
    vQueueDelete(gotten_data_holder);
    //gotten_data_holder = NULL; //class is destroyed: we don't need to do this

    //remove static lock
    some_initialized = false;

    //un-register esp-now
    esp_now_unregister_send_cb();
    esp_now_unregister_recv_cb();
    esp_now_deinit();
}


bool RadioNowHandler::add_peer(const char* peer_address, const char* key_peer) {


    esp_now_peer_info_t peer = {};// = {*peer_address, *key_s};

    memcpy(peer.peer_addr, peer_address, ESP_NOW_ETH_ALEN);
    memcpy(peer.lmk, key_peer, ESP_NOW_KEY_LEN);


    peer.channel = net_id;
    peer.encrypt = encrypted;

    if (esp_now_add_peer(&peer) != ESP_OK){
        Serial.println("Failed to add peer");
        return false;
    }
    return true;

}

TXStatus RadioNowHandler::SendPacket(RemoteGenericPacket packet, const char* peer_address) {

    if(!active || packet.get_transmission_len() > ESP_NOW_MAX_DATA_LEN)
        return TX_FAIL;

    auto size = packet.get_transmission_len();

    esp_err_t result = esp_now_send((const uint8_t*)peer_address, packet.get_transmission_ptr(), size);


    if(result)
        return TX_FAIL;

    return TX_SUCCESS;


}

RXStatus RadioNowHandler::CheckForPacket()
{
    //do not try if we're not enabled
    if(!active)
        return RX_FAIL;


    if(xQueueReceive(gotten_data_holder, raw_queue_dump, 0) == pdTRUE)
    {
        //wipes old object and replaces it with this one if successful
        packet = RemoteGenericPacket(raw_queue_dump);

        return RX_SUCCESS;

    }

    return RX_QUEUE_EMPTY;
}

RemoteGenericPacket RadioNowHandler::GetLastPacket() {
    return packet;
}

uint64_t RadioNowHandler::GetDeltaTime()
{
    if(!active)
        return 0;

    //get time difference
    //uint64_t delta_time = deltaTime(millis(), rec_time);
    return delta_time(millis(), rec_time);
}



