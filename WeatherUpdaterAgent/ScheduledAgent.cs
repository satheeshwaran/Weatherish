using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System.Device.Location;
using System.Windows.Controls;
using Windows.Devices.Geolocation;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Windows.Phone.System.UserProfile;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

using System.IO;
using System.Linq;

namespace WeatherUpdaterAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        public GeoCoordinate currentCoordinate;
        public Image currentWeatherIcon;
        public string dailyForecastData;
        public string weeklyForecastData;
        public string currentCity;
        public string currentPlace;
        public string currentCountry;
        private string flickrApiKey = "8244c11a9c3b02b45c127873ac225958";

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                loadIconicTileData();
                await getLocation();
                NotifyComplete();
            });

           
        }

        private void loadIconicTileData()
        {
            try
            {
                IconicTileData iconicTileData = new IconicTileData();
                iconicTileData.WideContent1 = "";
                iconicTileData.WideContent2 = "Test2";
                iconicTileData.WideContent3 = "Test3";
                iconicTileData.SmallIconImage = new Uri("/Assets/ApplicationIcon.png", UriKind.Relative);
                iconicTileData.IconImage = new Uri("/Assets/ApplicationIcon.png", UriKind.Relative);

                var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

                if (null != mainTile)
                {
                    mainTile.Update(iconicTileData);
                }
            }
            catch (Exception e)
            {

            }

        }

        private async Task getLocation()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition position =
                    await geolocator.GetGeopositionAsync(
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromSeconds(30));

                var gpsCoorCenter =
                    new GeoCoordinate(
                        position.Coordinate.Latitude,
                        position.Coordinate.Longitude);

                currentCoordinate = gpsCoorCenter;
                

                try
                {
                
                    await getCurrentPlaceAndWOEID();
                HttpClient weatherClient = new HttpClient();
                string weatherAPIURL = "http://api.openweathermap.org/data/2.5/weather?" + "lat=" + currentCoordinate.Latitude + "&lon=" + currentCoordinate.Longitude + "&units=metric";
               // string weatherAPIURL = "http://api.openweathermap.org/data/2.5/weather?lat=35&lon=139&units=metric";
                string weatherAPIResult = await weatherClient.GetStringAsync(weatherAPIURL);
                Console.WriteLine(weatherAPIResult);
               
                    string apiData = JsonConvert.DeserializeObject(weatherAPIResult).ToString();
                    RootObject apiDataJson = JsonConvert.DeserializeObject<RootObject>(apiData);
                    int temp = (int)apiDataJson.main.temp;

             
                   /*   currentTemperatureBlock.Text = temp.ToString() + "°C ";
                      currentTemperatureCondtion.Text = apiDataJson.weather[0].description;
                      hourlyForecastTextBlock.Text = DateTime.Now.ToString("MMM dd") + " Hourly Forecast";
                      currentTemperatureRangeBlock.Text = "Temp   " + apiDataJson.main.temp_min.ToString() + "°~" + apiDataJson.main.temp_max.ToString() + "°";
                      currentWindSpeedBlock.Text = apiDataJson.wind.speed.ToString() + " Kph";
                      currentHumidityTextBlock.Text = apiDataJson.main.humidity.ToString() + "%";

                      thisWeekForecastTitle.Text = ordinal(GetWeekOfMonth(DateTime.Now)) + " week of " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(DateTime.Now.Month);
                      */

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

                        if (null != mainTile)
                        {
                            mainTile.Update(CreateIconicTileData(currentPlace, apiDataJson.weather[0].description, apiDataJson.weather[0].icon, apiDataJson.main.temp.ToString()));
                        }

                        
                        var lockBackgroundImage = new Image
                        {
                            Source = new BitmapImage(new Uri("/Assets/Background.jpg", UriKind.RelativeOrAbsolute)),
                            Width = 480,
                            Height = 800
                        };

                        var lockWeatherImage = new Image
                        {
                            Source = new BitmapImage(new Uri(getImageURLForWeatherIcon(apiDataJson.weather[0].icon), UriKind.Relative)),
                            Width = 64,
                            Height = 64
                        };

                        var lockTextBlock = new TextBlock
                        {
                            Text = (currentPlace.Length>0?currentPlace:apiDataJson.name) + Environment.NewLine + Math.Round(apiDataJson.main.temp,2) + "°C" + Environment.NewLine +
                                    apiDataJson.weather[0].description,
                            FontSize = 24,
                            Foreground = new SolidColorBrush(Colors.White),
                            FontFamily = new FontFamily("Segoe WP SemiLight")
                        };

                        string fileName;
                        Uri currentImage;

                        try
                        {
                            currentImage = LockScreen.GetImageUri();
                        }
                        catch (Exception)
                        {
                            currentImage = new Uri("ms-appdata:///local/LiveLockBackground_A.jpg", UriKind.Absolute);
                        }

                        if (currentImage.ToString().EndsWith("_A.jpg"))
                        {
                            fileName = "LiveLockBackground_B.jpg";
                        }
                        else
                        {
                            fileName = "LiveLockBackground_A.jpg";
                        }

                        var lockImage = string.Format("{0}", fileName);
                        var isoStoreLockImage = new Uri(string.Format("ms-appdata:///local/{0}", fileName), UriKind.Absolute);

                        try
                        {
                        using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            var stream = store.CreateFile(lockImage);

                            var bitmap = new WriteableBitmap(480, 800);

                            bitmap.Render(lockBackgroundImage, new TranslateTransform());

                            bitmap.Render(lockWeatherImage, new TranslateTransform()
                            {
                                X = 25,
                                Y = 75
                            });

                            bitmap.Render(lockTextBlock, new TranslateTransform()
                            {
                                X = 25,
                                Y = 150
                            });

                            bitmap.Invalidate();
                            bitmap.SaveJpeg(stream, 480, 800, 0, 100);

                            stream.Close();

                        }
                        }
                        catch (Exception)
                        {
                            currentImage = new Uri("ms-appdata:///local/LiveLockBackground_A.jpg", UriKind.Absolute);
                        }


                        bool isProvider = LockScreenManager.IsProvidedByCurrentApplication;
                        if (isProvider)
                        {
                            LockScreen.SetImageUri(isoStoreLockImage);
                            System.Diagnostics.Debug.WriteLine("New current image set to {0}", isoStoreLockImage);
                        }

                        var toast = new ShellToast
                        {
                            Title = "Weatherish",
                            Content = "The lock screen was updated with new weather data",
                            NavigationUri = new Uri("/MainPage.xaml?agentLockscreen=1", UriKind.RelativeOrAbsolute)
                        };

                        toast.Show();

                    });

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
              
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                }
                //else
                {
                    // something else happened acquring the location
                }
            }

        }

        private async Task getCurrentPlaceAndWOEID()
        {
            try
            {
                HttpClient client = new HttpClient();

                string[] licenses = { "4", "5", "6", "7" };
                string license = String.Join(",", licenses);
                license = license.Replace(",", "%2C");
                double latitude = 0;
                double longitude = 0;

                if (!double.IsNaN(latitude))
                    latitude = Math.Round(latitude, 5);

                if (!double.IsNaN(longitude))
                    longitude = Math.Round(longitude, 5);

                // Your API key ... REPLACE THIS WITH YOURS:
                // http://www.flickr.com/services/api/keys/

                // Search API
                // http://www.flickr.com/services/api/flickr.photos.search.html

                string url = "http://api.flickr.com/services/rest/" +
                    "?method=flickr.places.findByLatLon" +
                    "&api_key={1}" +
                    "&lat=" + currentCoordinate.Latitude.ToString() +
                    "&lon=" + currentCoordinate.Longitude.ToString() +
                    "&format=json" +
                    "&nojsoncallback=1";

                var baseUrl = string.Format(url,
                    license,
                    flickrApiKey,
                    currentCoordinate.Latitude,
                    currentCoordinate.Longitude);

                string flickrResult = await client.GetStringAsync(baseUrl);
                Console.WriteLine(flickrResult);
                //ResultTextBlock.Text = flickrResult;

                Location apiData = JsonConvert.DeserializeObject<Location>(flickrResult);

                if (apiData.stat == "ok")
                {
                    foreach (Place placeObject in apiData.places.place)
                    {
                        currentPlace = placeObject.woe_name;
                        string[] cityName = placeObject.name.Split(',');
                        if (cityName.Length > 0)
                            currentCity = cityName[1];
                        if (cityName.Length > 3)
                            currentCountry = cityName[4];
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private ShellTileData CreateIconicTileData(string currentLocation,string weatherValue,string weatherIconValue, string weatherTemp)
        {
            IconicTileData iconincTile = new IconicTileData();
            iconincTile.SmallIconImage = new Uri(getImageURLForWeatherIcon(weatherIconValue), UriKind.Relative);
            iconincTile.WideContent1 = currentLocation;
            iconincTile.WideContent2 = weatherTemp + "°C";
            iconincTile.WideContent3 = weatherValue;

            return iconincTile;
        }

        public string getImageURLForWeatherIcon(string weatherIcon)
        {
            string localImageURL = "";
            if (weatherIcon.Equals("01d") || weatherIcon.Equals("01n"))
                localImageURL = "/Assets/WeatherIcons/weather-clear@2x.png";
            else if (weatherIcon.Equals("02d"))
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

    }

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Sys
    {
        public double message { get; set; }
        public string country { get; set; }
        public double sunrise { get; set; }
        public double sunset { get; set; }
    }

    public class Weather
    {
        public double id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double pressure { get; set; }
        public int humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public double deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class RootObject
    {
        public Coord coord { get; set; }
        public Sys sys { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public double dt { get; set; }
        public double id { get; set; }
        public string name { get; set; }
        public double cod { get; set; }
    }

    public class Place
    {
        public string place_id { get; set; }
        public string woeid { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string place_url { get; set; }
        public string place_type { get; set; }
        public int place_type_id { get; set; }
        public string timezone { get; set; }
        public string name { get; set; }
        public string woe_name { get; set; }
    }

    public class Places
    {
        public List<Place> place { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int accuracy { get; set; }
        public int total { get; set; }
    }

    public class Location
    {
        public Places places { get; set; }
        public string stat { get; set; }
    }
}