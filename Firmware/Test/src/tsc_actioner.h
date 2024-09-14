#pragma once

#include <Arduino.h>
#include <tsc_parser.h>
#include <radio_now_handler.h>
#include <configuration.h>



class TscActioner : public TscParser {

    public:

    TscActioner() {}
    ~TscActioner() {}

    //performs special actions based on what TSC we have (this will be different for each device type)
    //note: run parse_tsc before running this!
    void action_tsc(RadioNowHandler& radio_handler) {

        TscCommand command;
        while(pop_tsc_command(command)) {
            switch(command.type) {
                case TSC_SLT: {
                    //set light target
                    break;
                }
                case TSC_STS: {
                    break;
                }
                case TSC_SLC: {
                    break;
                }
                case TSC_SLR: {
                    break;
                }
                case TSC_SLS: {
                    break;
                }
                default: {
                    //forward command to "dongle" for now (example)
                    auto string_cmd = command.stringify_command();
                    RemoteGenericPacket packet = RemoteGenericPacket((uint8_t*)&string_cmd, string_cmd.size(), 0);
                    radio_handler.send_packet(packet, (char*)g_dongle_1.mac_address);
                    break;
                }
            }
        }

    }

};









