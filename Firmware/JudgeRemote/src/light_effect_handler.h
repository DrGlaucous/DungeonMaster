#pragma once

#include <Arduino.h>

#include <configuration.h>
#include <tsc_parser.h>




class LightEffectHandler {


    public:

    //calls actions on lights based on command
    void parse_command(TscCommand command) {

    }


    virtual void select_address_range(int min, int max) = 0;



};


enum LightPointState {
    LStateRunning,
    LStatePaused,
    LStateRunToPoint,
};

//A single light
struct LightPoint {
    
    //a list of colors to hit in sequential order
    vector<uint32_t> color_steps;
    uint32_t current_target;





};

//handles several LEDs connected directly to the ESP32
class MonoLedHandler : public LightEffectHandler {



    virtual void select_address_range(int min, int max) {

    }


};













