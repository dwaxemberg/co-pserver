using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game
{
    public static class Weather
    {
        public static DateTime NextChange = new DateTime();

        public static Network.GamePackets.Weather CurrentWeatherBase = new Conquer_Online_Server.Network.GamePackets.Weather(true);

        public static uint Intensity
        {
            get
            {
                return CurrentWeatherBase.Intensity;
            }
            set
            {
                CurrentWeatherBase.Intensity = value;
            }
        }

        public static uint Appearence
        {
            get
            {
                return CurrentWeatherBase.Appearence;
            }
            set
            {
                CurrentWeatherBase.Appearence = value;
            }
        }

        public static uint Direction
        {
            get
            {
                return CurrentWeatherBase.Direction;
            }
            set
            {
                CurrentWeatherBase.Direction = value;
            }
        }

        public static uint CurrentWeather
        {
            get
            {
                return CurrentWeatherBase.WeatherType;
            }
            set
            {
                CurrentWeatherBase.WeatherType = value;
                foreach (Client.GameState client in ServerBase.Kernel.GamePool.Values)
                {
                    CurrentWeatherBase.Send(client);
                }
            }
        }
    }
}
