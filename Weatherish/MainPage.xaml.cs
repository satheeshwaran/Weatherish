using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Net.Http;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Newtonsoft.Json;
using System.Globalization;

namespace Weatherish
{
    public partial class MainPage : PhoneApplicationPage
    {
        public GeoCoordinate currentCoordinate;
        public TextBlock currentTemperatureBlock;
        public TextBlock currentTemperatureCondtion;
        public TextBlock hourlyForecastTextBlock;
        public TextBlock thisWeekForecastTitle;
        public Image currentWeatherIcon;
        public string dailyForecastData;
        public string weeklyForecastData;
        public string currentCity;
        public string currentPlace;
        public string currentCountry;
        private string flickrApiKey = "8244c11a9c3b02b45c127873ac225958";

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData(dailyForecastData);
            }
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            getLocation();
        }

        private void Panorama_Loaded(object sender, RoutedEventArgs e)
        {

            ImageBrush b = new ImageBrush();
            BitmapImage image = new BitmapImage(new Uri("/Assets/bg.png", UriKind.Relative));
            b.ImageSource = image;
            b.Opacity = 0.70;
            weatherishPanorma.Background = b;
        }

        private async void getDailyForecastData()
        {
            HttpClient weatherClient = new HttpClient();
            string weatherAPIURL = "http://api.openweathermap.org/data/2.5/forecast?" + "lat=" + currentCoordinate.Latitude + "&lon=" + currentCoordinate.Longitude + "&units=metric";
            string weatherAPIResult = await weatherClient.GetStringAsync(weatherAPIURL);
            Console.WriteLine(weatherAPIResult);
            try
            {
                dailyForecastData = JsonConvert.DeserializeObject(weatherAPIResult).ToString();
                App.ViewModel.LoadData(dailyForecastData);
                
              
                //                TextBox foundTextBox = UIHelper.FindChild<TextBox>(longList, "currentTemperature");

            }
            catch (Exception ex)
            {

            }
            //var myTextBlock = (TextBlock)this.FindName("currentTemperature");
            //myTextBlock.Text = apiData.main.temp + "°";
        }
        

        private async void getWeeklyForecastData()
        {
            HttpClient weatherClient = new HttpClient();
            string weatherAPIURL = " http://api.openweathermap.org/data/2.5/forecast/daily?" + "lat=" + currentCoordinate.Latitude + "&lon=" + currentCoordinate.Longitude + "&units=metric" ;
            string weatherAPIResult = await weatherClient.GetStringAsync(weatherAPIURL);
            Console.WriteLine(weatherAPIResult);
            try
            {
                weeklyForecastData = JsonConvert.DeserializeObject(weatherAPIResult).ToString();
                App.ViewModel.loadWeeklyData(weeklyForecastData);                
            }
            catch (Exception ex)
            {

            }
            //var myTextBlock = (TextBlock)this.FindName("currentTemperature");
            //myTextBlock.Text = apiData.main.temp + "°";
        }

        private async void getCurrentWeatherData()
        {
            HttpClient weatherClient = new HttpClient();
            string weatherAPIURL = "http://api.openweathermap.org/data/2.5/weather?" + "lat=" + currentCoordinate.Latitude + "&lon=" + currentCoordinate.Longitude + "&units=metric" ;
            string weatherAPIResult = await weatherClient.GetStringAsync(weatherAPIURL);
            Console.WriteLine(weatherAPIResult);
            try
            {
                string apiData = JsonConvert.DeserializeObject(weatherAPIResult).ToString();
                RootObject apiDataJson = JsonConvert.DeserializeObject<RootObject>(apiData);
                int temp = (int)apiDataJson.main.temp;
                currentTemperatureBlock.Text = temp.ToString() + "°C ";
                currentTemperatureCondtion.Text = apiDataJson.weather[0].description;
                hourlyForecastTextBlock.Text = DateTime.Now.ToString("dd MMM yyyy") + "  Hourly Forecast";

                var currentCulture = CultureInfo.CurrentCulture;
                var weekNo = currentCulture.Calendar.GetWeekOfYear(
                                DateTime.Now,
                                currentCulture.DateTimeFormat.CalendarWeekRule,
                                currentCulture.DateTimeFormat.FirstDayOfWeek);
                BitmapImage image = new BitmapImage(new Uri(App.ViewModel.getImageURLForWeatherIcon(apiDataJson.weather[0].icon), UriKind.Relative));
                currentWeatherIcon.Source = image;
                    
//                TextBox foundTextBox = UIHelper.FindChild<TextBox>(longList, "currentTemperature");

            }
            catch (Exception ex)
            {

            }
            //var myTextBlock = (TextBlock)this.FindName("currentTemperature");
            //myTextBlock.Text = apiData.main.temp + "°";

        }
        private async void getLocation()
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
                getCurrentPlaceAndWOEID();
                getCurrentWeatherData();
                getDailyForecastData();
                getWeeklyForecastData();

            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    MessageBox.Show("location  is disabled in phone settings.");
                }
                //else
                {
                    // something else happened acquring the location
                }
            }

        }
        private async void getCurrentPlaceAndWOEID()
        {
            HttpClient client = new HttpClient();

            // About licenses:
            // http://www.flickr.com/services/api/flickr.photos.licenses.getInfo.html
            /*
             *  <license id="4" name="Attribution License" url="http://creativecommons.org/licenses/by/2.0/" />
                <license id="5" name="Attribution-ShareAlike License" url="http://creativecommons.org/licenses/by-sa/2.0/" />
                <license id="6" name="Attribution-NoDerivs License" url="http://creativecommons.org/licenses/by-nd/2.0/" />
                <license id="7" name="No known copyright restrictions" url="http://flickr.com/commons/usage/" />
             */
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
                "&api_key={1}"+
                "&lat=" +currentCoordinate.Latitude.ToString()+
                "&lon=" + currentCoordinate.Longitude.ToString()+
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
                foreach(Place placeObject in apiData.places.place)
                {
                    locationTextBlock.Text = placeObject.woe_name;
                    currentPlace = placeObject.woe_name;
                    string[] cityName = placeObject.name.Split(',');
                    if (cityName.Length > 0)
                        currentCity = cityName[1];
                    if (cityName.Length > 3)
                        currentCountry = cityName[4];
                    break;
                }
                getBGImageFromFlickrSource();

             }
        }
        private async void getBGImageFromFlickrSource()
        {

                HttpClient client = new HttpClient();

                string url = "http://api.flickr.com/services/rest/" +
                    "?method=flickr.groups.pools.getPhotos" +
                    "&api_key={0}" +
                    "&group_id=1463451%40N25"+
                    "&privacy_filter=1"+
                    "&tags="+currentPlace+
                    "&format=json" +
                    "&nojsoncallback=1";

                var baseUrl = string.Format(url,
                    flickrApiKey);

                string flickrResult = await client.GetStringAsync(baseUrl);
                Console.WriteLine(flickrResult);
                //ResultTextBlock.Text = flickrResult;

                FlickrData apiData = JsonConvert.DeserializeObject<FlickrData>(flickrResult);

                if (apiData.stat == "ok")
                {
                    if (apiData.photos.photo.Count > 0)
                    {
                        foreach (Photo data in apiData.photos.photo)
                        {
                            // To retrieve one photo, use this format:
                            //http://farm{farm-id}.staticflickr.com/{server-id}/{id}_{secret}{size}.jpg

                            string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg";
                            Console.WriteLine(photoUrl);

                            string baseFlickrUrl = string.Format(photoUrl,
                                data.farm,
                                data.server,
                                data.id,
                                data.secret);

                            setPanormaBG(baseFlickrUrl);
                            break;
                        }
                    }
                    else
                    {
                        getBGImageFromFlickrSourceForNearestCity();
                    }
                }           
        }

        private async void getBGImageFromFlickrSourceForNearestCity()
        {

            HttpClient client = new HttpClient();

            string url = "http://api.flickr.com/services/rest/" +
                "?method=flickr.groups.pools.getPhotos" +
                "&api_key={0}" +
                "&group_id=1463451%40N25" +
                "&privacy_filter=1" +
                "&tags=" + currentCity +
                "&format=json" +
                "&nojsoncallback=1";

            var baseUrl = string.Format(url,
                flickrApiKey);

            string flickrResult = await client.GetStringAsync(baseUrl);
            Console.WriteLine(flickrResult);
            //ResultTextBlock.Text = flickrResult;

            FlickrData apiData = JsonConvert.DeserializeObject<FlickrData>(flickrResult);

            if (apiData.stat == "ok")
            {
                if (apiData.photos.photo.Count > 0)
                {
                    Random r = new Random();
                    int rInt = r.Next(0, apiData.photos.photo.Count);
                    Photo data = apiData.photos.photo[rInt];

                    string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg";
                    Console.WriteLine(photoUrl);

                    string baseFlickrUrl = string.Format(photoUrl,
                        data.farm,
                        data.server,
                        data.id,
                        data.secret);

                    setPanormaBG(baseFlickrUrl);
                  
                }
                else
                {
                    getBGImageFromFlickrSourceForCountryAtleast();
                }
            }

            //AroundMeMap.SetView(new GeoCoordinate(41.8988D, -87.6231D), 17D);


        }

        private async void getBGImageFromFlickrSourceForCountryAtleast()
        {

            HttpClient client = new HttpClient();

            string url = "http://api.flickr.com/services/rest/" +
                "?method=flickr.groups.pools.getPhotos" +
                "&api_key={0}" +
                "&group_id=1463451%40N25" +
                "&privacy_filter=1" +
                "&tags=" + currentCountry +
                "&format=json" +
                "&nojsoncallback=1";

            var baseUrl = string.Format(url,
                flickrApiKey);

            string flickrResult = await client.GetStringAsync(baseUrl);
            Console.WriteLine(flickrResult);
            //ResultTextBlock.Text = flickrResult;

            FlickrData apiData = JsonConvert.DeserializeObject<FlickrData>(flickrResult);

            if (apiData.stat == "ok")
            {
                if (apiData.photos.photo.Count > 0)
                {
                    Random r = new Random();
                    int rInt = r.Next(0, apiData.photos.photo.Count);
                    Photo data = apiData.photos.photo[rInt];

                    string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg";
                    Console.WriteLine(photoUrl);

                    string baseFlickrUrl = string.Format(photoUrl,
                        data.farm,
                        data.server,
                        data.id,
                        data.secret);

                    setPanormaBG(baseFlickrUrl);

                }
            }

            //AroundMeMap.SetView(new GeoCoordinate(41.8988D, -87.6231D), 17D);


        }

        private void setPanormaBG(string imgURL)
        {
             ImageBrush b = new ImageBrush();
                    b.ImageSource = new BitmapImage(new Uri(imgURL));
                    b.Opacity = 0.70;
                    b.Stretch = Stretch.Fill;
                    weatherishPanorma.Background = b;
        }

        private void textBoxLoaded(object sender, RoutedEventArgs e)
        {
            currentTemperatureBlock = (TextBlock)sender;
        }

        private void currentTemperatureCondtion_Loaded(object sender, RoutedEventArgs e)
        {
            currentTemperatureCondtion = (TextBlock)sender;
        }

        private void hourlyForecastTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            hourlyForecastTextBlock = (TextBlock)sender;
        }

        private void thisWeekForecastTitle_Loaded(object sender, RoutedEventArgs e)
        {
            thisWeekForecastTitle = (TextBlock)sender;
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            currentWeatherIcon = (Image)sender;
        }   


    }

    public class Photo
    {
        public string id { get; set; }
        public string owner { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public int farm { get; set; }
        public string title { get; set; }
        public int ispublic { get; set; }
        public int isfriend { get; set; }
        public int isfamily { get; set; }
    }

    public class Photos
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public string total { get; set; }
        public List<Photo> photo { get; set; }
    }

    public class FlickrData
    {
        public Photos photos { get; set; }
        public string stat { get; set; }
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