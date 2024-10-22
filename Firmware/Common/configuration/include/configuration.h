#pragma once

//Configuration
#include <FastLED.h>
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

struct DirectLedPins {
    int r_pin;
    int g_pin;
    int b_pin;
};

enum PacketType {
    PacketTscCommand = 1,
    PacketResponse = 2,
};

enum ResponseType {
    PacketGetOk = 0,
    ButtonStatus = 1,
};

//events to run, tied with button IDs
enum ButtonEventNumbers {
    //controller enums
    ButtonAActive = 1000,
    ButtonBActive = 1001,
    ButtonCActive = 1002,
    ButtonDActive = 1003,
    ButtonEActive = 1004,
    ButtonFActive = 1005,
    ButtonGActive = 1006,
    ButtonHActive = 1007,
    ButtonAInactive = 2000,
    ButtonBInactive = 2001,
    ButtonCInactive = 2002,
    ButtonDInactive = 2003,
    ButtonEInactive = 2004,
    ButtonFInactive = 2005,
    ButtonGInactive = 2006,
    ButtonHInactive = 2007,


    BlueReadyInactive = 2007,
    ArenaDoorInactive = 2008,
};

////////RADIO SETTINGS////////

//espnow
#define NETWORKID           4   // Must be the same for all nodes

#define ENCRYPT       true // Set to "true" to use encryption
//peer encryption key (we're re-using this for each peer, though we might be able to use custom ones)
#define ENCRYPTKEY_PEER {0xA0,0xA0,0xFF,0x00,0xFF,0xA0,0xA0,0xA0,0xFF,0x45,0xA0,0xA0,0x26,0x20,0x43,0xA0} //local master
#define ENCRYPTKEY_NETWORK    "JACKBLACKISSTEVE" //"TOPSECRETPASSWRD" // Use the same 16-byte key on all nodes


//global define variables

//note: {0xA1, 0xEE, 0xF4, 0x2F, 0xF0, 0x64}, is considered a multicast address for some reason...

const DeviceInfo g_dongle_1 = {
    {0x30, 0xAE, 0xA4, 0x07, 0x0D, 0x64}, //MAC address
    ENCRYPTKEY_PEER, //peer localkey
    TypeDongle, //device type
    0, //uuid
};
const DeviceInfo g_jcontroller_1 = {
    {0x20, 0xEE, 0x04, 0x2F, 0xF0, 0x64},
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


////////LED SETTINGS////////

//the same pin for both divices since this must be checked at compile-time
//pin that drives the neopixel LED strips
#define STRIP_LED_DRIVER_PIN 16

//the driver type for the LED strips
#define DRIVER_TYPE NEOPIXEL



//remote

//the remote doesn't have any of these for now
#define REMOTE_RGB_LED_COUNT 0

const DirectLedPins remote_led_addresses[] =  {
    DirectLedPins{4,-1,-1}, //0
    DirectLedPins{25 -1,-1}, //1
    DirectLedPins{16, -1,-1}, //2
    DirectLedPins{26,-1,-1}, //3
    DirectLedPins{17,-1,-1}, //4
    DirectLedPins{12,-1,-1}, //5
    DirectLedPins{5,-1,-1}, //6
    DirectLedPins{13,-1,-1}, //7
};

//box
#define BOX_RGB_LED_COUNT 120 //number neopixel-type leds within the box, addresses start where the box_led_addresses end
//2 leds for each "ready button"
const DirectLedPins box_led_addresses[] =  {
    DirectLedPins{4,-1,-1}, //0
    DirectLedPins{25,-1,-1}, //1
};



//global method (this doesn't really fit anywhere else...)
RemoteGenericPacket assemble_response_packet(size_t device_type, size_t device_id, ResponseType response_type, const char* data) {

    string payload = "<";
    payload += std::to_string(device_type) + ':' + std::to_string(device_id) + ':' + std::to_string((int)response_type) + ':';

    //note: data should be null-terminated!
    payload += data;

    return RemoteGenericPacket((const uint8_t*)payload.c_str(), payload.size() + 1, PacketResponse);
}
