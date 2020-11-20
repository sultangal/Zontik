using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WeatherProviderConsoleApp
{
    static class ConsoleMessage
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static public void Write(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Logger.Info(message);
        }
        static public void Write(string message, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Logger.Info(message);
            Logger.Error(e);
        }
    }
}
