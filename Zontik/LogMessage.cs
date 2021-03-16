using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zontik
{
    static class LogMessage
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static public void Write(string message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
#endif
            Logger.Info(message);
        }
        static public void Write(string message, Exception e)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
#endif
            Logger.Info(message);
            Logger.Error(e);
        }
    }
}
