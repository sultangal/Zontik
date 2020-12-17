﻿using System;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Zontik
{
    class WeatherGis : Weather
    {
        private GisMeteoWeatherAPI gisMeteoWeatherAPI;
        public WeatherGis(string lat, string lon)
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

        public override int WeatherTemp()
        {
            return (int)Math.Truncate(gisMeteoWeatherAPI.data.temperature.air.C);
        }

        public override string WeatherCondition()
        {
            return ConvertGisIconID(gisMeteoWeatherAPI.data.icon.IconWeather.ToString());
        }

        private string ConvertGisIconID(string iconId)
        {

            if (iconId.Contains("st"))                                                              return "storm";
            else if (iconId.Contains("r1") && iconId.Contains("r2") && iconId.Contains("r3"))       return "rain";
            else if (iconId.Contains("s1") && iconId.Contains("s2") && iconId.Contains("s3"))       return "snow";
            else if (iconId.Contains("rs1") && iconId.Contains("rs2") && iconId.Contains("rs3"))    return "sleet";
            else if (iconId.Contains("mist"))                                                       return "mist";
            else if (iconId.Contains("c1") && iconId.Contains("c2") && iconId.Contains("c101"))     return "clouds";
            else if (iconId.Contains("c3"))                                                         return "overcast";
                                                                                                    return "clear";
            //switch (iconId)
            //{
            //    case iconId.Contains("st"):
            //        Console.WriteLine("Case 1");
            //        break;
            //    case 2:
            //        Console.WriteLine("Case 2");
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}
            //return "null";
        }

    }
}
