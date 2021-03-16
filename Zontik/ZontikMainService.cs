using System;
using System.Collections.Generic;
using CronNET;
using System.Reflection;
using System.Threading;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Zontik
{
    class ZontikMainService
    //: ServiceBase
    {
        private static readonly CronDaemon cron_daemon = new CronDaemon();
        private static object _lock = new object();
        public ZontikMainService() { }
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
            new Thread(new ThreadStart(EngineMonitoring)).Start();
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Time_Loop"], Task);
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Clear_Old_Logs"], ClearOldLogs);
            cron_daemon.Start();
        }

        private void Task()
        {
            lock (_lock)
            {
                try
                {
                    string host = "127.0.0.1";
                    int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                    int cityCount = 0;
                    SendToVizEngine sendToVizEngine = new SendToVizEngine();
                    XmlReadItem xmlReadItem = new XmlReadItem(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\city_list.xml");
                    List<Item> item_list = xmlReadItem.ReturnItemList();
                    LogMessage.Write("Список городов считан из файла xml");
                    Item item = new Item();
                    string lat = "";
                    string lon = "";
                    string val = "";

                    for (int i = 0; i < item_list.Count; i++) //
                    {
                        string city = item_list[i].City;
                        lat = item_list[i].Lat.ToString();
                        lon = item_list[i].Lon.ToString();

                        string tempStr;
                        IWeather weather = new WeatherGis(lat, lon);
                        if (weather.isError == true) continue;
                        int temp = weather.WeatherTemp();
                        if (temp > 0) tempStr = "+" + temp; else tempStr = temp.ToString(); //adding sign "+" to temperature above zero
                        string condition = weather.WeatherCondition();
                        LogMessage.Write("(GIS)Начинаю передачу следующих данных в VizEngine:");
                        LogMessage.Write($"{cityCount,5} {lat,5} {lon,20} {tempStr,20} {city,5} {condition,20} " +
                            $"{"currDate " + weather.LocalCurrDate(),10} {"frcDate " + weather.LocalForecastDate(),10}");
                        val = city + "*" + tempStr + "*" + condition;
                        sendToVizEngine.SendViaTCP(host, port, "key" + cityCount, val);
                        cityCount++;
                    }
                    LogMessage.Write(cityCount.ToString());

                    if (cityCount != 0)
                    {
                        sendToVizEngine.SendViaTCP(host, port, "city_number", cityCount.ToString()); //
                        DateTime now = DateTime.Now;
                        LogMessage.Write((now.Hour * 60 + now.Minute).ToString());
                        sendToVizEngine.SendViaTCP(host, port, "data_freshness", (now.Hour * 60 + now.Minute).ToString());                       
                    }

                }
                catch (Exception e)
                {
                    LogMessage.Write("Произошла ошибка. Перезапустите службу.", e);
                }
            }
            
        }

        private void EngineMonitoring()
        {
            try
            {
                while (true)
                {
                    //ConsoleMessage.Write("Мониторю...");                   
                    Process[] proc = Process.GetProcessesByName("viz");
                    if (proc.Length != 0)
                    {
                        //ConsoleMessage.Write("Engine врубился. Подписываюсь на событие Exited");
                        proc[0].EnableRaisingEvents = true;
                        proc[0].Exited += new EventHandler(EventDoOnEngineExit);
                        break;                        
                    }
                    Thread.Sleep(5000);
                }
            }

            catch (Exception e)
            {
                LogMessage.Write("Произошла ошибка в процессе мониторинга VizEngine.", e);
            }
                                     
        }

        private void EventDoOnEngineExit(object sender, EventArgs e)
        {
            //ConsoleMessage.Write("Engine exited!! Врубай заново!!");
            Task();  
            EngineMonitoring();
        }

        private void ClearOldLogs()
        {
         try {
                string[] folderName = { "\\logs", "\\logs\\errors" };
                for (int i = 0; i < folderName.Length; i++)
                {
                    if (Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + folderName[i]))
                    {
                        string[] logFolder = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + folderName[i], "*.log");
                        foreach (string file in logFolder)
                        {
                            FileInfo fi = new FileInfo(file);
                            if (fi.LastWriteTime < DateTime.Now.AddMonths(-1))
                                fi.Delete();
                        }
                    }
                }                              
                LogMessage.Write("Очистка логов прошла успешно");

            }
            catch (Exception e)
            {
                LogMessage.Write("Ошибка очистки логов", e);
            }
}
    }
}
