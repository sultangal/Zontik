using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using CronNET;
using System.Threading;
using System.Net.Sockets;
using Topshelf;

namespace Zontik
{
    class Program
    {
        private static readonly CronDaemon cron_daemon = new CronDaemon();
        private static readonly string host = System.Configuration.ConfigurationManager.AppSettings["host"];
        private static readonly int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
        static void Main(string[] args)
        {
            Task();
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Time_Loop"], Task);
            cron_daemon.Start();
            
            //Wait and sleep forever. Let the cron daemon run.
            while (true) Thread.Sleep(6000);
            //Console.ReadLine();


            var exitCode = HostFactory.Run(x =>
            {
                x.Service<H>
            });
        }

         static void Task()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(System.DateTime.Now);
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
                Console.WriteLine((now.Hour*60+now.Minute).ToString()); 
                sendToVizEngine.SendViaTCP(host, port, "data_freshness", (now.Hour * 60 + now.Minute).ToString());
            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Произошла ошибка", e);
            }
        }
    }
}
