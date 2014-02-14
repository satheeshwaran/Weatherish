using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Weatherish
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();

            if (!System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("EnableLocation"))
                EnableLocation.IsChecked = true;
            else
                EnableLocation.IsChecked = (bool?)System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["EnableLocation"] ?? true;
        }

        private void EnableLocation_Checked(object sender, RoutedEventArgs e)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["EnableLocation"] = true;
        }

        private void EnableLocation_Unchecked(object sender, RoutedEventArgs e)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings["EnableLocation"] = false;
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            new EmailComposeTask
            {
                Subject="Regarding Weatherish",
                Body = "Kindly key in your feedback or questions",
                To = "satmobdev@live.in"
            }.Show();
        }
    }
}