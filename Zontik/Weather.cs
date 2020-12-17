using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zontik
{
    abstract class Weather
    {
        abstract public int WeatherTemp();
        abstract public string WeatherCondition();
    }
}
