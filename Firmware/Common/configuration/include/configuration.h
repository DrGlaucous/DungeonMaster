//Configuration

//#include <esp_now.h>

////////RADIO SETTINGS////////

//espnow
#define NETWORKID           4   // Must be the same for all nodes

//custom MAC addresses
#define DONGLE_ADDRESS {0x30, 0xAE, 0xA4, 0x07, 0x0D, 0x64}
#define BOX_ADDRESS {0xA0, 0x0E, 0x04, 0x0F, 0xFD, 0x64}
#define JUDGE_CONTROLLER_ADDRESS {0xA1, 0xEE, 0xF4, 0x2F, 0xF0, 0x64}

#define ENCRYPT       true // Set to "true" to use encryption
//peer encryption key (we're re-using this for each peer, though we might be able to use custom ones)
#define ENCRYPTKEY_PEER {0xA0,0xA0,0xFF,0x00,0xFF,0xA0,0xA0,0xA0,0xFF,0x45,0xA0,0xA0,0x26,0x20,0x43,0xA0}
#define ENCRYPTKEY_NETWORK    "JACKBLACKISSTEVE" //"TOPSECRETPASSWRD" // Use the same 16-byte key on all nodes


//device UUIDs (used for command addressing)
#define DONGLE1_UUID 0
#define JUDGE_CONTROLLER_UUID 1
#define BOX_UUID 2


////////////ENUM DEFINITIONS///////////////

//NOT the UUID, but rather the type of device this is
enum DeviceType {
    TypeDongle = 0,
    TypeJudgeController = 1,
    TypeBox = 2,
}


struct DeviceInfo {

}







