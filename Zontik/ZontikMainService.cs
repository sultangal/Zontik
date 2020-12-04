using System;
using System.Collections.Generic;
using CronNET;
using System.Reflection;
using System.Threading;
using System.ServiceProcess;
using System.Threading.Tasks;

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
            Thread task_thread = new Thread(new ThreadStart(TaskNewThread));
            task_thread.Start();

        }

        public void OnStop()
        {
            cron_daemon.Stop();
        }


        private void TaskNewThread()
        {
            Task();
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Time_Loop"], Task);
            cron_daemon.Start();

        }

        private void Task()
        {
            try
            {
                string host = System.Configuration.ConfigurationManager.AppSettings["host"];
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);

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
                    ConsoleMessage.Write("Начинаю передачу следующих данных в VizEngine:");
                    ConsoleMessage.Write(i + "\t" + lat + " " + lon + "\t" + temp + "\t" + city.Trim() + " \t \t " + condition.Trim());
                    val = city + "*" + temp + "*" + condition;
                    sendToVizEngine.SendViaTCP(host, port, "key" + i, val);
                }
                ConsoleMessage.Write(item_list.Count.ToString());
                sendToVizEngine.SendViaTCP(host, port, "city_number", item_list.Count.ToString()); //
                DateTime now = DateTime.Now;
                ConsoleMessage.Write((now.Hour * 60 + now.Minute).ToString());
                sendToVizEngine.SendViaTCP(host, port, "data_freshness", (now.Hour * 60 + now.Minute).ToString());
            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Произошла ошибка. Перезапустите службу.", e);
            }
        }
    }
}
