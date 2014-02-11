using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Weatherish.Resources;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;


namespace Weatherish.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.dailyForecastItems = new ObservableCollection<ItemViewModel>();
            this.weeklyForecastItems = new ObservableCollection<ItemViewModel>();

        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> dailyForecastItems { get; private set; }
        public ObservableCollection<ItemViewModel> weeklyForecastItems { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData(string dailyForecastData)
        {
            if (dailyForecastData != null && dailyForecastData.Length >0)
            {
                try
                {
                
                DailyWeatherObject foreCastJSONData = JsonConvert.DeserializeObject<DailyWeatherObject>(dailyForecastData);
                this.dailyForecastItems.Clear();
                    foreach (WeatherList listObj in foreCastJSONData.list)
                    {
                        DateTime forecastTime = FromUnixTime(listObj.dt);
                        if(forecastTime.Date == DateTime.Today.Date)
                            this.dailyForecastItems.Add(new ItemViewModel() { dayTime = forecastTime.ToShortTimeString(), weatherDescription = listObj.weather[0].description, temperature = listObj.main.temp.ToString() + "°C", temperatureMinMaxValue = listObj.main.temp_max.ToString() + "°C" + "/" + listObj.main.temp_min.ToString() + "°C",weatherImageURL = getImageURLForWeatherIcon(listObj.weather[0].icon) });
                    }
                    this.IsDataLoaded = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }


            }
        }

           public void loadWeeklyData(string dailyForecastData)
        {
            if (dailyForecastData != null && dailyForecastData.Length > 0)
            {
                try
                {

                    WeeklyWeatherObject foreCastJSONData = JsonConvert.DeserializeObject<WeeklyWeatherObject>(dailyForecastData);
                    this.weeklyForecastItems.Clear();
                    foreach (WeeklyWeatherList listObj in foreCastJSONData.list)
                    {
                        DateTime forecastTime = FromUnixTime(listObj.dt);
                        //if (forecastTime.Date == DateTime.Today.Date)
                        this.weeklyForecastItems.Add(new ItemViewModel() { dayTime = forecastTime.DayOfWeek.ToString(), weatherDescription = listObj.weather[0].description, temperature = listObj.temp.day.ToString() + "°C", temperatureMinMaxValue = listObj.temp.max.ToString() + "°C" + "/" + listObj.temp.min.ToString() + "°C", weatherImageURL = getImageURLForWeatherIcon(listObj.weather[0].icon) });
                    }
                    this.IsDataLoaded = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
            }
        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            DateTime returnDate = epoch.AddSeconds(unixTime);
            return returnDate;
        }

        public string getImageURLForWeatherIcon(string weatherIcon)
        {
            string localImageURL = "";
            if(weatherIcon.Equals("01d") || weatherIcon.Equals("01n"))
                localImageURL = "/Assets/WeatherIcons/weather-clear@2x.png";
            else if(weatherIcon.Equals("02d"))
                localImageURL = "/Assets/WeatherIcons/weather-few@2x.png";
            else if (weatherIcon.Equals("02n"))
                localImageURL = "/Assets/WeatherIcons/weather-few-night@2x.png";
            else if (weatherIcon.Equals("03d") || weatherIcon.Equals("03n"))
                localImageURL = "/Assets/WeatherIcons/weather-scattered@2x.png";
            else if (weatherIcon.Equals("04d") || weatherIcon.Equals("04n"))
                localImageURL = "/Assets/WeatherIcons/weather-broken@2x.png";
            else if (weatherIcon.Equals("09d") || weatherIcon.Equals("09n"))
                localImageURL = "/Assets/WeatherIcons/weather-shower@2x.png";
            else if (weatherIcon.Equals("10d"))
                localImageURL = "/Assets/WeatherIcons/weather-rain@2x.png";
            else if (weatherIcon.Equals("10n"))
                localImageURL = "/Assets/WeatherIcons/weather-rain-night@2x.png";
            else if (weatherIcon.Equals("11d") || weatherIcon.Equals("11n"))
                localImageURL = "/Assets/WeatherIcons/weather-tstorm@2x.png";
            else if (weatherIcon.Equals("13d") || weatherIcon.Equals("13n"))
                localImageURL = "/Assets/WeatherIcons/weather-snow@2x.png";
            else if (weatherIcon.Equals("50d") || weatherIcon.Equals("50n"))
                localImageURL = "/Assets/WeatherIcons/weather-mist@2x.png";
            return localImageURL;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class City
    {
        public double id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public double population { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double pressure { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
        public double humidity { get; set; }
        public double temp_kf { get; set; }
    }

    public class Weather
    {
        public double id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Clouds
    {
        public double all { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public double deg { get; set; }
    }

    public class Sys
    {
        public string pod { get; set; }
    }

    public class Rain
    {
        public double __invalid_name__3h { get; set; }
    }

    public class WeatherList
    {
        public long dt { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        //public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        //public Sys sys { get; set; }
        public string dt_txt { get; set; }
        //public Rain rain { get; set; }
    }

    public class DailyWeatherObject
    {
        public string cod { get; set; }
        public double message { get; set; }
        public City city { get; set; }
        public double cnt { get; set; }
        public List<WeatherList> list { get; set; }
    }


public class Temp
{
    public double day { get; set; }
    public double min { get; set; }
    public double max { get; set; }
    public double night { get; set; }
    public double eve { get; set; }
    public double morn { get; set; }
}


public class WeeklyWeatherList
{
    public int dt { get; set; }
    public Temp temp { get; set; }
    public double pressure { get; set; }
    public int humidity { get; set; }
    public List<Weather> weather { get; set; }
    public double speed { get; set; }
    public int deg { get; set; }
    public int clouds { get; set; }
}

public class WeeklyWeatherObject
{
    public string cod { get; set; }
    public double message { get; set; }
    public City city { get; set; }
    public int cnt { get; set; }
    public List<WeeklyWeatherList> list { get; set; }
}
}