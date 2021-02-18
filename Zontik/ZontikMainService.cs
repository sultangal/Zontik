using System;
using System.Collections.Generic;
using CronNET;
using System.Reflection;
using System.Threading;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.IO;

namespace Zontik
{
    class ZontikMainService
    //: ServiceBase
    {
        private static readonly CronDaemon cron_daemon = new CronDaemon();

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
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Time_Loop"], Task);
            cron_daemon.AddJob(System.Configuration.ConfigurationManager.AppSettings["Clear_Old_Logs"], ClearOldLogs);
            cron_daemon.Start();
        }

        private void Task()
        {
            try
            {
                string host = System.Configuration.ConfigurationManager.AppSettings["host"];
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                //int apiSwitch = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["API_switch"]);
                int cityCount = 0;
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
                    string city = item_list[i].City;
                    lat = item_list[i].Lat.ToString();
                    lon = item_list[i].Lon.ToString();

                    //switch (apiSwitch)
                    //{
                    //    case 1:
                    //        {
                                string tempStr;
                                WeatherGis weather = new WeatherGis(lat, lon);
                                if (weather.isError == true) continue;                                
                                int temp = weather.WeatherTemp();
                                if (temp > 0) tempStr = "+" + temp; else tempStr = temp.ToString(); //adding sign "+" to temperature above zero
                                string condition = weather.WeatherCondition();
                                ConsoleMessage.Write("(GIS)Начинаю передачу следующих данных в VizEngine:");
                                ConsoleMessage.Write($"{cityCount,5} {lat,5} {lon,20} {tempStr,20} {city,5} {condition,20} {"currDate "+weather.LocalCurrDate(),10} {"frcDate "+weather.LocalForecastDate(),10}");
                                val = city + "*" + tempStr + "*" + condition;
                                sendToVizEngine.SendViaTCP(host, port, "key" + cityCount, val);
                                cityCount++;
                    //            break;
                    //        }
                    //    case 2:
                    //        { 
                    //            WeatherYan weather = new WeatherYan(lat, lon);
                    //            int temp = weather.WeatherTemp();
                    //            string condition = weather.WeatherCondition();
                    //            ConsoleMessage.Write("(YAN)Начинаю передачу следующих данных в VizEngine:");
                    //            ConsoleMessage.Write($"{i,40} {lat,5} {lon,20} {temp,20} {city,5} {condition,20}");
                    //            val = city + "*" + temp + "*" + condition;
                    //            sendToVizEngine.SendViaTCP(host, port, "key" + i, val);
                    //            break;
                    //        }
                    //    default:
                    //        break;
                    //}

                    //if (apiSwitch == 1)
                    //{
                    //    WeatherGis weather = new WeatherGis(lat, lon);
                    //    int temp = weather.WeatherTemp();
                    //    string condition = weather.WeatherCondition();
                    //    ConsoleMessage.Write("Начинаю передачу следующих данных в VizEngine:");
                    //    ConsoleMessage.Write($"{i,40} {lat,5} {lon,20} {temp,20} {city,5} {condition,20}");
                    //    val = city + "*" + temp + "*" + condition;
                    //    sendToVizEngine.SendViaTCP(host, port, "key" + i, val);
                    //}
                    //else if (apiSwitch == 2)
                    //{
                    //    WeatherYandex weather = new WeatherYandex(lat, lon);
                    //    int temp = weather.WeatherTemp();
                    //    string condition = weather.WeatherCondition();
                    //    ConsoleMessage.Write("Начинаю передачу следующих данных в VizEngine:");
                    //    ConsoleMessage.Write($"{i,40} {lat,5} {lon,20} {temp,20} {city,5} {condition,20}");
                    //    val = city + "*" + temp + "*" + condition;
                    //    sendToVizEngine.SendViaTCP(host, port, "key" + i, val);
                    //}

                }
                ConsoleMessage.Write(cityCount.ToString());
                if (cityCount != 0) 
                {
                    sendToVizEngine.SendViaTCP(host, port, "city_number", cityCount.ToString()); //
                    DateTime now = DateTime.Now;
                    ConsoleMessage.Write((now.Hour * 60 + now.Minute).ToString());
                    sendToVizEngine.SendViaTCP(host, port, "data_freshness", (now.Hour * 60 + now.Minute).ToString());
                } 
                
            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Произошла ошибка. Перезапустите службу.", e);
            }
        }

        private void ClearOldLogs()
        {
         try { 
                string[] logFolder = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs","*.log");
                string[] errFolder = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\errors", "*.log");
                foreach (string file in logFolder)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddMonths(-1)) 
                        fi.Delete();
                }
                foreach (string file in errFolder)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddMonths(-1)) 
                        fi.Delete();
                }
                ConsoleMessage.Write("Очистка логов прошла успешно");

            }
            catch (Exception e)
            {
                ConsoleMessage.Write("Ошибка очистки логов", e);
            }
}
    }
}
