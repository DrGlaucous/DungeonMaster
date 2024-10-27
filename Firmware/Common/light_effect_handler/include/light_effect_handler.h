#pragma once

#include <vector>

#include <Arduino.h>

#include <configuration.h>
#include <tsc_parser.h>

#include <FastLED.h>

using std::vector;

// enum LightPointState {
//     LStateRunning,
//     LStatePaused,
//     LStateRunToPoint,
// };


//an instruction in the lightpoint's list
struct ColorStep {
    //color to hit
    byte rgb[3];
    //time it takes to go from current color to this one
    uint32_t time_ms;
};

enum LightPointType {
    TypeDirect,
    TypeStrip,
};

//A single light with R, G, and B componets
class LightPoint {
    

    public:

    LightPoint(int id): id(id) {}

    virtual ~LightPoint() {};

    //sets the output of this light
    virtual void set_out() = 0;

    //advances the color fading
    void tick(uint32_t delta_t_ms) {

        //don't do anything if there are no color targets present
        if(!color_steps.size()) {
            return;
        }

        //Serial.printf("Tick light: %d\n", id);

        curr_time += delta_t_ms;

        //set to next target
        while(curr_time >= color_steps[current_target].time_ms) {
            
            //Serial.printf("Q1\n");
            
            curr_time -= color_steps[current_target].time_ms;

            //increment target index
            size_t last_target = current_target;
            ++current_target;
            if(current_target >= color_steps.size()) {
                current_target -= color_steps.size();
            }

            //Serial.printf("R\n");

            //cache last color
            const byte* last_col = color_steps[last_target].rgb;
            memcpy(last_rgb, last_col, 3);

        }

        //map out linear colors
        const byte* next_rgb = color_steps[current_target].rgb;

        rgb[0] = map(curr_time, 0, color_steps[current_target].time_ms, last_rgb[0], next_rgb[0]);
        rgb[1] = map(curr_time, 0, color_steps[current_target].time_ms, last_rgb[1], next_rgb[1]);
        rgb[2] = map(curr_time, 0, color_steps[current_target].time_ms, last_rgb[2], next_rgb[2]);


        //Serial.printf("Col: %d, %d, %d\n", rgb[0], rgb[1], rgb[2]);
        //Serial.printf("%d | %d | %d \n", rgb[0], rgb[1], rgb[2]);

    }

    //reset the "curr_time" timer to 0 to restart the light fade at this index
    inline void reset_timer() {
        curr_time = 0;
    }

    //reset the index back to 0
    inline void reset_index() {
        current_target = 0;
    }

    //empty color step vector
    inline void clear_color_steps() {
        color_steps.clear();
        reset_index();
    }

    //add a new step to the color step vector
    void push_color_step(ColorStep step) {
        color_steps.push_back(step);
    }

    //set an existing color step to a new value
    bool set_color_Step(ColorStep step, size_t index) {
        if(index >= color_steps.size()) {
            return false;
        }

        color_steps[index] = step;

        return true;
    }

    //get immutable refrence to current color
    inline const byte* get_rgb() {
        return rgb;
    }

    inline uint32_t get_id() {
        return id;
    }

    virtual LightPointType get_type() = 0;

    private:

    //id of the light, used for indexing
    int id;

    //a list of colors to hit in sequential order
    vector<ColorStep> color_steps;

    //index of the current color to hit
    size_t current_target = 0;

    //where we are between 0 and the end time (from color_steps)
    uint32_t curr_time = 0;

    //last color that was hit, used to map between old and new colors
    byte last_rgb[3] = {0,0,0};

    //output color to be written to the corresponding LED
    byte rgb[3] = {0,0,0};

};

//A single light whose output is controlled directly from the microcontroller
class DirectLed : public LightPoint {

    public:

    DirectLed(int id, int r_out, int g_out, int b_out): LightPoint(id), r_out(r_out), g_out(g_out), b_out(b_out) {
    
        //Serial.printf("Setup id: %d\n", id);
        if(r_out >= 0) {
            //Serial.printf("R: %d\n", r_out);
            pinMode(r_out, OUTPUT);
        }
        if(g_out >= 0) {
            //Serial.printf("G: %d\n", g_out);
            pinMode(g_out, OUTPUT);
        }
        if(b_out >= 0) {
            //Serial.printf("B: %d\n", b_out);
            pinMode(b_out, OUTPUT);
        }
    
    }

    //push to output
    void set_out() {

        const byte* rgb_ptr = get_rgb();

        if(r_out >= 0) {
            analogWrite(r_out, rgb_ptr[0]);
        }
        if(g_out >= 0) {
            analogWrite(g_out, rgb_ptr[1]);
        }
        if(b_out >= 0) {
            analogWrite(b_out, rgb_ptr[2]);
        }
        
    }

    LightPointType get_type() { return LightPointType::TypeDirect; }

    private:

    int r_out = -1;
    int g_out = -1;
    int b_out = -1;


};

//A single light whose output is controlled using the fastLED library
class StripLed : public LightPoint {


    public:

    StripLed(int id, CRGB* color_ref): LightPoint(id), color_ref(color_ref) {}

    //push to output
    void set_out() {

        const byte* rgb_ptr = get_rgb();

        color_ref->r = rgb_ptr[0];
        color_ref->g = rgb_ptr[1];
        color_ref->b = rgb_ptr[2];
    }

    LightPointType get_type() { return LightPointType::TypeStrip; }

    private:

    //refrence to a point in a CRGB array which corresponds to this light
    CRGB* color_ref;


};

//template<class DriverType>
class LightEffectHandler {


    public:

    //initialize with a list of both strip leds and direct leds
    LightEffectHandler(int strip_led_count, const DirectLedPins* direct_led_array, int direct_led_count) {

        int total_count = strip_led_count + direct_led_count;
        int id = 0; //address index since strip lights will be addressed after direct lights

        point_list.reserve(total_count);

        //init direct lights
        if(direct_led_count > 0) {
            for(int i = 0; i < direct_led_count; ++i) {
                LightPoint* led = new DirectLed(
                    id,
                    direct_led_array[i].r_pin,
                    direct_led_array[i].g_pin,
                    direct_led_array[i].b_pin
                    );
                
                ++id;

                point_list.push_back(led);
            }
        }

        //init fastLED lights
        //todo: make the protocol here dynamic...
        //also note: the pin number here must also be a constant... fun traits....
        if(strip_led_count > 0) {
            fast_led_colors.resize(strip_led_count);

            FastLED.addLeds<NEOPIXEL, STRIP_LED_DRIVER_PIN>(&fast_led_colors[0], strip_led_count);

            for(int i = 0; i < strip_led_count; ++i) {
                LightPoint* led = new StripLed(id, &fast_led_colors[i]);
                point_list.push_back(led);
                ++id;
            }
        }

    }

    ~LightEffectHandler() {

        //free all light points
        for(int i = 0; i < point_list.size(); ++i) {
            delete(point_list[i]);
        }

        //is this the best way to deinitialize this?
        FastLED.clear();
    }


    //tick each led's color
    void tick(uint32_t delta_t_ms) {
        for(int i = 0; i < point_list.size(); ++i) {
            point_list[i]->tick(delta_t_ms);
        }
    }

    //applies calculated colors to output
    void set_out() {

        for(int i = 0; i < point_list.size(); ++i) {
            point_list[i]->set_out();
        }

        //if we have strip lights, write them all out
        if(fast_led_colors.size() > 0) {
            FastLED.show();
        }

    }


    //calls actions on lights based on command
    void parse_command(TscCommand &command) {

        switch(command.type) {
            case TSC_SLT: {
                //start, stop, step

                if(command.command_count < 3) {
                    break;
                }

                start = command.tsc_args[0];
                stop = command.tsc_args[1];
                step = command.tsc_args[2];

                //handle improper args
                if(stop <= start) {
                    stop = start + 1;
                }
                if(step <= 0) {
                    step = 1;
                }

                Serial.printf("RAN: SLT\n");

                break;
            }

            case TSC_PLC: {
                
                if(command.command_count < 4) {
                    break;
                }

                //set the color of lights within the proper ID range
                for(int i = 0; i < point_list.size(); ++i) {
                    
                    if(is_within_range(point_list[i]->get_id())) {
                        //set parameter

                        ColorStep new_color = ColorStep{
                            {
                                (byte)clamp(command.tsc_args[0], 0, 255),
                                (byte)clamp(command.tsc_args[1], 0, 255),
                                (byte)clamp(command.tsc_args[2], 0, 255),
                            },
                            command.tsc_args[3] < 1 ? 1 : (uint32_t)command.tsc_args[3] //prevent 0-delta lighting functions
                        };

                        point_list[i]->push_color_step(new_color);
                    }


                }

                Serial.printf("RAN: PLC\n");

                break;
            }        
            case TSC_CLC: {

                for(int i = 0; i < point_list.size(); ++i) {
                    if(is_within_range(point_list[i]->get_id())) {
                        point_list[i]->clear_color_steps();
                    }
                }

                Serial.printf("RAN: CLC\n");                
                
                break;
            }
            case TSC_RLI: {

                for(int i = 0; i < point_list.size(); ++i) {
                    if(is_within_range(point_list[i]->get_id())) {
                        point_list[i]->reset_index();
                    }
                }
                break;
            }
            case TSC_RLT: {

                for(int i = 0; i < point_list.size(); ++i) {
                    if(is_within_range(point_list[i]->get_id())) {
                        point_list[i]->reset_timer();
                    }
                }
                break;
            }
            case TSC_SLR: {

                if(command.command_count < 1) {
                    break;
                }

                int time_ms = command.tsc_args[0];

                int selected_count = 0;

                //get total number of lights we'll be affecting
                for(int i = 0; i < point_list.size(); ++i) {
                    if(is_within_range(point_list[i]->get_id())) {
                        ++selected_count;
                    }
                }

                int current_idx = 0;

                //map time offset to each of those
                for(int i = 0; i < point_list.size(); ++i) {
                    if(is_within_range(point_list[i]->get_id())) {
                        int time_offset = map(current_idx, 0, selected_count, 0, time_ms);
                        point_list[i]->tick(time_offset);
                        ++current_idx;
                        //Serial.printf("TO: %d CI: %d, SC: %d, TM: %d\n", time_offset, current_idx, selected_count, time_ms);
                    }
                }

                Serial.printf("RAN: SLR\n");   

                break;
            }
        
        }

    }



    private:

    bool is_within_range(int id) {

        if(id < start
        || id >= stop
        || (id - start) % step) {
            return false;
        }

        return true;
    }

    inline int clamp(int input, int min, int max) {
        return input > max ? max : (input < min ? min : input);
    }


    //list of all points to be controlled by this light effect handler
    vector<LightPoint*> point_list;

    //an array of colors for the strip lights
    vector<CRGB> fast_led_colors;

    
    //parameters to select LED addresses
    int start = 0; //inclusive
    int stop = 1; //exclusive
    int step = 1;



};





