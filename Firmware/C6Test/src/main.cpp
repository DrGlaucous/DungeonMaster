
#include <Arduino.h>

#include <ESP32Servo.h>
#include <Adafruit_NeoPixel.h>
#include <math.h>


//angle when the flywheels are "off", also serves as the arming angle
#define ESC_IDLE_ANGLE 37
//angle when the flywheels are "max" (actually, they max out somewhat below this, I think somewhere around 150?)
#define ESC_MAX_ANGLE 180

#define ARM_DELAY 5750



//onboard led type: WS2812

#define PIN       8 //onboard LED pin
#define NUMPIXELS 1

//Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

Servo servo1;
Servo servo2;
Servo servo3;
Servo servo4;

//good pins to use:
/*

18,19,21,22

*/



void setup()
{
    //pinMode(2, OUTPUT);

    servo1.attach(18);
    servo2.attach(19);
    servo3.attach(21);
    servo4.attach(22);

    Serial.begin(115200);
    Serial.print("ready\n");


    servo1.write(ESC_IDLE_ANGLE);
    servo2.write(ESC_IDLE_ANGLE);
    servo3.write(ESC_IDLE_ANGLE);
    servo4.write(ESC_IDLE_ANGLE);
    delay(ARM_DELAY);

    Serial.printf("Waiting...\n");
    while(!Serial.available()) {
        delay(1);
    }

    Serial.printf("Starting motors...\n");

}

void loop()
{

    servo1.write(60);
    servo2.write(60);
    servo3.write(60);
    servo4.write(60);

    //very quick and diry "Rainbow" effect with the onboard RGB LED
    // float ratio_r = sin((float)millis() / 100) * 1000;
    // float ratio_g = sin((float)(millis() + 1000) / 100) * 1000;
    // float ratio_b = sin((float)(millis() + 2000) / 100) * 1000;

    // auto r_color = map((int)ratio_r, -1000, 1000, 0, 255);
    // auto g_color = map((int)ratio_g, -1000, 1000, 0, 255);
    // auto b_color = map((int)ratio_b, -1000, 1000, 0, 255);

    //pixels.setPixelColor(0, pixels.Color(r_color, g_color, b_color));
    //pixels.show();



}

