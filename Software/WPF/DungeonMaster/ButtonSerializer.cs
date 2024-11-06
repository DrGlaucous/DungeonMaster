using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DungeonMaster
{
    internal class EventButton
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int Event { get; set; }
        public EventButton()
        {
            Name = "";
            Color = "#000000";
        }

        public (byte, byte, byte) ColorAsRgb()
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;

            return (0, 0, 0);
        }
        public void ColorFromRGB(byte r, byte g, byte b)
        {

        }
    }
    internal class EventButtonJson
    { 
        public List<EventButton> Buttons { get; set; }

        public EventButtonJson() { 
            Buttons = new List<EventButton>();
        }
    }
    internal class EventButtonSerializer
    {
        private EventButtonJson buttons = new EventButtonJson();

        public EventButtonSerializer() {

            var eventButton = new EventButton();
            eventButton.Name = "AAA";
            eventButton.Color = "#606060";
            eventButton.Event = 0;

            buttons.Buttons.Append(eventButton);


            string jsonString = JsonSerializer.Serialize(buttons);
        }

        public EventButtonJson GetButtonList() {
            return buttons;
        }

    }
}
