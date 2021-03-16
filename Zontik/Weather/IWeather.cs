using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zontik
{
    interface IWeather
    {
        public bool isError { get; } 
        public int WeatherTemp();
        public string WeatherCondition();
        public string LocalCurrDate();
        public string LocalForecastDate();        
    }
}
