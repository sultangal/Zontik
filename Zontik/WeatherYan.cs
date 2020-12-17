using System;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Zontik
{
    class WeatherYan : Weather
    {
        private YandexWeatherAPI yandexWeatherAPI;
        public WeatherYan(string lat, string lon)
        {
            while (true)
            {
                try
                {
                    lat = lat.Replace(",", ".");
                    lon = lon.Replace(",", ".");
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.weather.yandex.ru/v2/forecast?lat={lat}&lon={lon}");
                    request.Headers.Add("X-Yandex-API-Key: " + System.Configuration.ConfigurationManager.AppSettings["X-Yandex-API-Key"]);

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

        public override int WeatherTemp()
        {
            return yandexWeatherAPI.fact.temp;
        }

        public override string WeatherCondition()
        {
            return ConvertYandexeIconID(yandexWeatherAPI.fact.condition.ToString());
        }

        private string ConvertYandexeIconID(string iconId)
        {
            if      (iconId == "thunderstorm" && 
                     iconId == "thunderstorm-with-rain" && 
                     iconId == "thunderstorm-with-hail")        return "storm";
            else if (iconId == "showers" && 
                     iconId == "continuous-heavy-rain" && 
                     iconId == "heavy-rain" && 
                     iconId == "moderate-rain" && 
                     iconId == "rain" && 
                     iconId == "light-rain" &&
                     iconId == "hail" &&
                     iconId == "drizzle")                       return "rain";
            else if (iconId == "snow-showers" && 
                     iconId == "snow" && 
                     iconId == "light-snow")                    return "snow";
            else if (iconId == "wet-snow")                      return "sleet";
          //else if (iconId.Contains("mist"))                   return "mist";
            else if (iconId == "cloudy" && 
                     iconId == "partly-cloudy")                 return "clouds";
            else if (iconId == "overcast")                      return "overcast";
            else if (iconId == "clear")                         return "clear";
            return "clear";
        }
    }
}