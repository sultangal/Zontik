using Newtonsoft.Json;
using System.Collections.Generic;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
public class Meta
{
    public bool status { get; set; }
    public int status_code { get; set; }
}

public class Jsonapi
{
    public string version { get; set; }
}

public class City
{
    public string name { get; set; }
    public string nameP { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

public class Icon
{
    [JsonProperty("icon-weather")]
    public string IconWeather { get; set; }
    public string emoji { get; set; }
}

public class Max
{
    public double C { get; set; }
    public double degree { get; set; }
    public int scale_8 { get; set; }
    public int m_s { get; set; }
}

public class Min
{
    public double C { get; set; }
    public double degree { get; set; }
    public int scale_8 { get; set; }
    public int m_s { get; set; }
}

public class Avg
{
    public double C { get; set; }
    public double degree { get; set; }
    public int scale_8 { get; set; }
    public double m_s { get; set; }
}

public class Air
{
    public Max max { get; set; }
    public Min min { get; set; }
    public Avg avg { get; set; }
}

public class Comfort
{
    public Max max { get; set; }
    public Min min { get; set; }
}

public class Water
{
    public Max max { get; set; }
    public Min min { get; set; }
}

public class Temperature
{
    public Air air { get; set; }
    public Comfort comfort { get; set; }
    public Water water { get; set; }
}

public class Precipitation
{
    public int type { get; set; }
    public int type_ext { get; set; }
    public double amount { get; set; }
    public int intensity { get; set; }
}

public class MmHgAtm
{
    public int min { get; set; }
    public int max { get; set; }
}

public class Pressure
{
    public MmHgAtm mm_hg_atm { get; set; }
}

public class Storm
{
    public double cape { get; set; }
    public bool prediction { get; set; }
}

public class Date
{
    public int unix { get; set; }
    public string UTC { get; set; }
    public string local { get; set; }
    public int timeZoneOffset { get; set; }
}

public class Direction
{
    public Max max { get; set; }
    public Min min { get; set; }
    public Avg avg { get; set; }
}

public class Speed
{
    public Max max { get; set; }
    public Min min { get; set; }
    public Avg avg { get; set; }
}

public class GustSpeed
{
    public Max max { get; set; }
}

public class Wind
{
    public Direction direction { get; set; }
    public Speed speed { get; set; }
    public GustSpeed gust_speed { get; set; }
}

public class Cloudiness
{
    public int percent { get; set; }
    public int scale_3 { get; set; }
}

public class Percent
{
    public int max { get; set; }
    public int min { get; set; }
    public double avg { get; set; }
}

public class Humidity
{
    public Percent percent { get; set; }
}

public class Data
{
    public string kind { get; set; }
    public City city { get; set; }
    public Icon icon { get; set; }
    public string description { get; set; }
    public Temperature temperature { get; set; }
    public Precipitation precipitation { get; set; }
    public Pressure pressure { get; set; }
    public Storm storm { get; set; }
    public Date date { get; set; }
    public Cloudiness cloudiness { get; set; }
    public Humidity humidity { get; set; }
}

public class GisMeteoWeatherAPI
{
    public List<Data> data { get; set; }
}



