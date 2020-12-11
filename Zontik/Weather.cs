using System;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Zontik
{
    class Weather
    {
        //private YandexWeatherAPI yandexWeatherAPI;
        private GisMeteoWeatherAPI gisMeteoWeatherAPI;
        public Weather(string lat, string lon)
        {
            while (true) { 
                try
                {           
                lat = lat.Replace(",", ".");
                lon = lon.Replace(",", ".");               
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://api.gismeteo.net/v3/weather/current?latitude={lat}&longitude={lon}");
                request.Headers.Add("X-Gismeteo-Token:" + System.Configuration.ConfigurationManager.AppSettings["X-Gismeteo-Token"]);

                    WebProxy myproxy = new WebProxy(System.Configuration.ConfigurationManager.AppSettings["ProxyServerIp"],
                        Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ProxyServerPort"]))
                    {
                        BypassProxyOnLocal = false
                    };
                    request.Proxy = myproxy;
                    request.Method = "GET";

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    ConsoleMessage.Write("Получен ответ от сервера погоды");
                string strResponse;
                using (StreamReader streamRead = new StreamReader(response.GetResponseStream()))
                {
                    strResponse = streamRead.ReadToEnd();
                }
                    ConsoleMessage.Write("Полученный поток успешно прочитан");

                    gisMeteoWeatherAPI = JsonConvert.DeserializeObject<GisMeteoWeatherAPI>(strResponse);
                }
                catch (Exception e)
                {
                    ConsoleMessage.Write("Ошибка запроса на сервер погоды. Попробую снова через минуту...", e);
                    Thread.Sleep(60000);
                    continue;
                }
               if (gisMeteoWeatherAPI != null) { break; }
            }

        }

        public int WeatherTemp()
        {
            return (int)Math.Truncate(gisMeteoWeatherAPI.data.temperature.air.C);
        }

        public string WeatherCondition()
        {
            return gisMeteoWeatherAPI.data.icon.IconWeather.ToString();
        }
    }
}
