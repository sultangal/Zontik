using System;
using System.Collections.Generic;
using CronNET;
using System.Reflection;
using System.Threading;
using System.ServiceProcess;

namespace Zontik
{
    class ZontikMainService 
        //: ServiceBase
    {
        private static readonly CronDaemon cron_daemon = new CronDaemon();
        
        public ZontikMainService()
        {
           
        }
        public void OnStart()
        {
            //Task();
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Time_Loop"], Task);
            cron_daemon.Start();
        }

        public void OnStop()
        {
            cron_daemon.Stop();
        }
        private void Task()
        {
            string host = System.Configuration.ConfigurationManager.AppSettings["host"];
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            try
            {
                //Console.ForegroundColor = ConsoleColor.White;
                //Console.WriteLine(System.DateTime.Now);
                SendToVizEngine sendToVizEngine = new SendToVizEngine();
                XmlReadItem xmlReadItem = new XmlReadItem(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\city_list.xml");
                List<Item> item_list = xmlReadItem.ReturnItemList();
                ConsoleMessage.Write("Список городов считан из файла xml");
                Item item = new Item();
                string lat = "";
                string lon = "";
                string val = "";
                for (int i = 0; i < item_list.Count; i++) //
                {
                    item = item_list.Find(a => a.Index == i);
                    string city = item.City;
                    lat = item.Lat.ToString();
                    lon = item.Lon.ToString();
                    Weather weather = new Weather(lat, lon);
                    int temp = weather.WeatherTemp();
                    string condition = weather.WeatherCondition();
                    ConsoleMessage.Write("Передаю данные");
                    ConsoleMessage.Write(i + "\t" + lat + " " + lon + "\t" + temp + "\t" + city.Trim() + " \t \t " + condition.Trim());
                    val = city + "*" + temp + "*" + condition;
                    sendToVizEngine.SendViaTCP(host, port, "key" + i, val);
                }
                sendToVizEngine.SendViaTCP(host, port, "city_number", item_list.Count.ToString()); //
                DateTime now = DateTime.Now;
                //Console.WriteLine((now.Hour * 60 + now.Minute).ToString());
                sendToVizEngine.SendViaTCP(host, port, "data_freshness", (now.Hour * 60 + now.Minute).ToString());
            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Произошла ошибка", e);
            }
        }
    }
}
