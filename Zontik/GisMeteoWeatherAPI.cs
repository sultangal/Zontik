using Newtonsoft.Json;

public class Air
{
    public double C { get; set; }
}

public class Temperature
{
    public Air air { get; set; }

}

public class Icon
{
    [JsonProperty("icon-weather")]
    public string IconWeather { get; set; }
}

public class Data
{
    
    public Temperature temperature { get; set; }
    public Icon icon { get; set; }

}

public class GisMeteoWeatherAPI
{

    public Data data { get; set; }
}

