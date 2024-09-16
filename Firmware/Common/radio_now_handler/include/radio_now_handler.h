#pragma once
#include <Arduino.h>

#include <esp_now.h>
#include <esp_wifi.h>
#include <Wifi.h>

using std::swap;

enum RXStatus
{
    RX_FAIL = 0,
    RX_SUCCESS = 1,
    RX_QUEUE_EMPTY,

};
enum TXStatus
{
    TX_FAIL = 0,
    TX_SUCCESS = 1,
};


// struct peer_data {
//     char mac_address[ESP_NOW_ETH_ALEN] = {};
//     size_t uuid = 0; //unique identifier that goes with this MAC address
// }


//#pragma pack(1) //no need to pack: we store everything in a char array
class RemoteGenericPacket {

    public:

    //takes payload + length of payload in bytes + type
    RemoteGenericPacket(const uint8_t* in_data, size_t len, size_t type) {

        //uninitialized
        if(in_data == NULL || len == 0) {
            InitializeEmpty();
            return;
        }

        //make holder of proper size
        this->data = (uint8_t*)calloc(len + sizeof(type), 1);
        this->len = len + sizeof(type);
        memcpy(this->data, &type, sizeof(type));
        memcpy(this->data + sizeof(type), in_data, len);

    }

    //assumes data is in the form [len_of_packet_and_type + type + data]
    RemoteGenericPacket(uint8_t* const in_data) {

        if(in_data == NULL) {
            InitializeEmpty();
            return;
        }
        
        //get the length of the packet we have (raw packet length)
        int len = *in_data;

        //clamp potential OOB values
        if(len > max_packet_len) {
            len = max_packet_len;
        }
        

        //copy raw packet into memory
        this->data = (uint8_t*)malloc(len);
        this->len = len;
        memcpy(this->data, in_data + sizeof(int), len);
    }

    //must also re-malloc our heap-allocated array
    RemoteGenericPacket(const RemoteGenericPacket& other) {
        len = other.len;
        data = (uint8_t*)malloc(len);
        memcpy(data, other.data, len);
    }

    RemoteGenericPacket() {
        InitializeEmpty();
    }

    ~RemoteGenericPacket() {

        if(this->data != NULL)
            free(this->data);
    }

    //copy + swap method
    RemoteGenericPacket& operator=(const RemoteGenericPacket& other) {
        
        auto holder = other;
        swap(len, holder.len);
        swap(data, holder.data);

        return *this;
    }

    //the total size we will be sending out, the length of the pointer in get_ready_packet()
    size_t get_transmission_len() const{
        return len;
    }
    //size of the payload (minus header)
    size_t get_data_len() const{
        return len - sizeof(size_t);
    }
    //return constant pointer to all the data to be transmitted, including header
    const uint8_t* get_transmission_ptr() {
        return data;
    }
    //return constant pointer to where the actual payload starts, minus header
    const uint8_t* get_data_ptr() {
        return data + sizeof(size_t);
    }

    //return the header of the packet
    size_t get_packet_type() {
        return (size_t)*data;
    }


    private:

    //make a packet with only the "type" inside it
    void InitializeEmpty() {

        //protect against memory leaks
        if(data != NULL)
            free(data);

        //add the size of the "type" value + 1 extra for a null terminator in the data section
        data = (uint8_t*)malloc(sizeof(size_t) + 1);
        memset(data, 0, sizeof(size_t) + 1);
        len = sizeof(size_t) + 1; 
    }

    //max size of the actual payload (size_t bytes are taken by the type)
    const static size_t max_packet_len = ESP_NOW_MAX_DATA_LEN;

    //the size of the payload data ("data"). Note that this value is not transmitted; it's only used for memcpy operations
    //max len is max_packet_len
    size_t len = sizeof(size_t);

    //data format: [size_t type][uint8_t* payload], we keep the type and payload concatanated in here
    uint8_t* data = NULL;

};

class RadioNowHandler
{
    public:
    RadioNowHandler(
        uint32_t net_id, //must be the same for all units on the network
        const char* key_p, //main key (not encrypted if NULL)
        const char* my_address
    );
    ~RadioNowHandler();


    bool add_peer(const char* peer_address, const char* key_s);

    //if peer_address is null, the message is broadcast
    TXStatus send_packet(RemoteGenericPacket packet, const char* peer_address);


    RemoteGenericPacket get_last_packet();
    RXStatus check_for_packet();

    //returns the time between when the packet was actually gotten and when this function was called
    //because packets can sit in the queue for a bit
    uint64_t get_delta_time();


    private:
    
    //network ID we constructred ourselves on
    uint32_t net_id = 0;
    bool encrypted = false;

    //intialize this in the cpp file
    static bool some_initialized;

    //if we tried to construct this and succeeded (this class instance will work)
    bool active = false;


    //raw buffer where the queue contents go
    uint8_t raw_queue_dump[ESP_NOW_MAX_DATA_LEN + sizeof(int)] = {};

    //where the packet goes if decode is successful
    RemoteGenericPacket packet;


};




