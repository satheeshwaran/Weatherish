using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Weatherish.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _dayTime;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string dayTime
        {
            get
            {
                return _dayTime;
            }
            set
            {
                if (value != _dayTime)
                {
                    _dayTime = value;
                    NotifyPropertyChanged("dayTime");
                }
            }
        }

        private string _weatherDescription;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string weatherDescription
        {
            get
            {
                return _weatherDescription;
            }
            set
            {
                if (value != _weatherDescription)
                {
                    _weatherDescription = value;
                    NotifyPropertyChanged("weatherDescription");
                }
            }
        }

        private string _temperature;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                if (value != _temperature)
                {
                    _temperature = value;
                    NotifyPropertyChanged("temperature");
                }
            }
        }

        private string _temperatureMinMaxValue;
        public string temperatureMinMaxValue
        {
            get
            {
                return _temperatureMinMaxValue;
            }
            set
            {
                if (value != _temperatureMinMaxValue)
                {
                    _temperatureMinMaxValue = value;
                    NotifyPropertyChanged("temperatureMinMaxValue");
                }
            }
        }

        private string _weatherImageURL;

        public string weatherImageURL
        {
            get
            {
                return _weatherImageURL;
            }
            set
            {
                if (value != _weatherImageURL)
                {
                    _weatherImageURL = value;
                    NotifyPropertyChanged("weatherImageURL");
                }
            }
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

   
}