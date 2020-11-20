using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            try
            {           
            lat = lat.Replace(",", ".");
            lon = lon.Replace(",", ".");
            string url = ($"https://api.weather.yandex.ru/v2/forecast?lat={lat}&lon={lon}");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Yandex-API-Key: "+System.Configuration.ConfigurationManager.AppSettings["X-Yandex-API-Key"]);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                ConsoleMessage.Write("Получен ответ от сервера погоды");
            string strResponse;
            using (StreamReader streamRead = new StreamReader(response.GetResponseStream()))
            {
                strResponse = streamRead.ReadToEnd();
            }
                ConsoleMessage.Write("Поток успешно прочитан");

            yandexWeatherAPI = JsonConvert.DeserializeObject<YandexWeatherAPI>(strResponse);
            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Ошибка запроса на сервер погоды", e);
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
