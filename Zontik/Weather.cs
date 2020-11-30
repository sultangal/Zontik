using System;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Zontik
{
    class Weather
    {
        private YandexWeatherAPI yandexWeatherAPI;
        public Weather(string lat, string lon)
        {
            while (true) { 
                try
                {           
                lat = lat.Replace(",", ".");
                lon = lon.Replace(",", ".");           
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.weather.yandex.ru/v2/forecast?lat={lat}&lon={lon}");
                request.Headers.Add("X-Yandex-API-Key: "+System.Configuration.ConfigurationManager.AppSettings["X-Yandex-API-Key"]);

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

                yandexWeatherAPI = JsonConvert.DeserializeObject<YandexWeatherAPI>(strResponse);
                }
                catch (Exception e)
                {
                    ConsoleMessage.Write("Ошибка запроса на сервер погоды. Попробую снова через минуту...", e);
                    Thread.Sleep(60000);
                    continue;
                }
               if (yandexWeatherAPI != null) { break; }
            }

        }

        public int WeatherTemp()
        {
            return yandexWeatherAPI.fact.temp;
        }

        public string WeatherCondition()
        {
            return yandexWeatherAPI.fact.condition.ToString();
        }
    }
}
