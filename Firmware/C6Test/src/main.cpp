
#include <Arduino.h>

#include <ESP32Servo.h>
#include <Adafruit_NeoPixel.h>
#include <math.h>


//onboard led type: WS2812

#define PIN        8 //onboard LED pin
#define NUMPIXELS 1

Adafruit_NeoPixel pixels(NUMPIXELS, PIN, NEO_GRB + NEO_KHZ800);

void setup()
{

    //pinMode(2, OUTPUT);

    Serial.begin(115200);
    Serial.print("ready\n");

    pixels.begin();
}

void loop()
{

    //very quick and diry "Rainbow" effect with the onboard RGB LED

    float ratio_r = sin((float)millis() / 100) * 1000;
    float ratio_g = sin((float)(millis() + 1000) / 100) * 1000;
    float ratio_b = sin((float)(millis() + 2000) / 100) * 1000;

    auto r_color = map((int)ratio_r, -1000, 1000, 0, 255);
    auto g_color = map((int)ratio_g, -1000, 1000, 0, 255);
    auto b_color = map((int)ratio_b, -1000, 1000, 0, 255);

    pixels.setPixelColor(0, pixels.Color(r_color, g_color, b_color));
    pixels.show();
    delay(10);

}

