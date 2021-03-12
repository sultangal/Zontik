using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Zontik
{
    public class SendToVizEngine
    {
        public SendToVizEngine() { }
        public void SendViaTCP(string host, int port, string key, string val)
        {
            using TcpClient _tcpClient = new TcpClient();
            string data = key + "|" + EscapeString(val);
            while (true)
            {
                try
                {
                    ConsoleMessage.Write("Подключаюсь к VizEngine...");
                    _tcpClient.Connect(host, port);
                    ConsoleMessage.Write("Подключение успешно установлено");
                    _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    _tcpClient.Client.Send(Encoding.UTF8.GetBytes(data + "\0"));
                    ConsoleMessage.Write("Данные успешно переданы в VizEngine \n");
                }
                catch (Exception e)
                {
                    ConsoleMessage.Write("Ошибка подключения к VizEngine. Попробую снова через 15 секунд...", e);
                    Thread.Sleep(15000);
                    continue;
                }

                if (_tcpClient.Connected)
                {
                    _tcpClient.Close();
                    break;
                };
            }
        }

        private char GetHexDigit(uint i)
        {
            if (i <= 9)
                return Convert.ToChar(Convert.ToUInt32('0') + i);
            else
                return Convert.ToChar(Convert.ToUInt32('A') + i - 10);
        }

        private string EscapeString(string s)
        {
            string returnStr = "";
            int i = 0;
            char a;

            for (i = 0; i < s.Length; ++i)
            {
                a = s[i];

                if (Char.IsLetterOrDigit(a) || a == '.' || a == '-' || a == '+')
                    returnStr += a;
                else
                {
                    uint digit = (uint)a;
                    returnStr += '$';
                    returnStr += GetHexDigit((digit >> 4) & 15u);
                    returnStr += GetHexDigit(digit & 15u);
                }
            }
            return returnStr;
        }

    }



}


