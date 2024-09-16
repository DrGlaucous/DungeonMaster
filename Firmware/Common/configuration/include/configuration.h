#pragma once

//Configuration

#include <esp_now.h>
#include <string>


using std::string;

////////////TYPE DEFINITIONS///////////////

//NOT the UUID, but rather the type of device this is
enum DeviceType {
    TypeDongle = 0,
    TypeJudgeController = 1,
    TypeBox = 2,
};


struct DeviceInfo {
    char mac_address[ESP_NOW_ETH_ALEN];
    char local_master_key[ESP_NOW_KEY_LEN];
    DeviceType type;
    size_t uu_id;
};


enum PacketType {
    PacketTscCommand = 1,
    PacketResponse = 2,
};

enum ResponseType {
    PacketGetOk = 0,
    ButtonStatus = 1,
};

////////RADIO SETTINGS////////

//espnow
#define NETWORKID           4   // Must be the same for all nodes

#define ENCRYPT       true // Set to "true" to use encryption
//peer encryption key (we're re-using this for each peer, though we might be able to use custom ones)
#define ENCRYPTKEY_PEER {0xA0,0xA0,0xFF,0x00,0xFF,0xA0,0xA0,0xA0,0xFF,0x45,0xA0,0xA0,0x26,0x20,0x43,0xA0} //local master
#define ENCRYPTKEY_NETWORK    "JACKBLACKISSTEVE" //"TOPSECRETPASSWRD" // Use the same 16-byte key on all nodes

//device UUIDs (used for command addressing)
#define DONGLE1_UUID 0
#define JUDGE_CONTROLLER1_UUID 1
#define BOX1_UUID 2


//global define variables

const DeviceInfo g_dongle_1 = {
    {0x30, 0xAE, 0xA4, 0x07, 0x0D, 0x64}, //MAC address
    ENCRYPTKEY_PEER, //peer localkey
    TypeDongle, //device type
    0, //uuid
};
const DeviceInfo g_jcontroller_1 = {
    {0xA1, 0xEE, 0xF4, 0x2F, 0xF0, 0x64},
    ENCRYPTKEY_PEER,
    TypeJudgeController,
    1,
};
const DeviceInfo g_box_1 = {
    {0xA0, 0x0E, 0x04, 0x0F, 0xFD, 0x64},
    ENCRYPTKEY_PEER,
    TypeBox,
    2,
};

//global method (this doesn't really fit anywhere else...)
RemoteGenericPacket assemble_response_packet(size_t device_type, size_t device_id, ResponseType response_type, const char* data) {

    string payload = "<";
    payload += std::to_string(device_type) + ':' + std::to_string(device_id) + ':' + std::to_string((int)response_type) + ':';

    //note: data should be null-terminated!
    payload += data;

    return RemoteGenericPacket((const uint8_t*)payload.c_str(), payload.size() + 1, PacketResponse);
}
