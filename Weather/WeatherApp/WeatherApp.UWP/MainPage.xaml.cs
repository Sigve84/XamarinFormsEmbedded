﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Weather.Forms;
using Xamarin.Forms;
using Page = Windows.UI.Xaml.Controls.Page;

namespace WeatherApp.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

			// Create a XF History page and drop it into a flyout from the command bar
	        var x = new HistoryPage().CreateFrameworkElement();
			HistoryFlyout.Content = x;

			// Listen for lookup requests from the history tracker
			MessagingCenter.Subscribe<HistoryPage, string>(this, HistoryPage.HistoryItemSelected, (history, postalCode) =>
			{
				Flyout.Hide();
				SetPostalCode(postalCode);
			});
		}

		public static readonly DependencyProperty IsHistoryOpenProperty = DependencyProperty.RegisterAttached("IsHistoryOpen", typeof(bool), typeof(Flyout), null);

		public async void SetPostalCode(string postalCode)
		{
			zipCodeEntry.Text = postalCode;
			await GetWeather();
		}

	    private async Task GetWeather()
	    {
			if (!String.IsNullOrEmpty(zipCodeEntry.Text))
			{
				Weather weather = await Core.GetWeather(zipCodeEntry.Text);
				if (weather != null)
				{
					locationText.Text = weather.Title;
					tempText.Text = weather.Temperature;
					windText.Text = weather.Wind;
					visibilityText.Text = weather.Visibility;
					humidityText.Text = weather.Humidity;
					sunriseText.Text = weather.Sunrise;
					sunsetText.Text = weather.Sunset;

					weatherBtn.Content = "Search Again";

					// Let the history tracker know that the user just successfully looked up a postal code
					var item = new HistoryItem(zipCodeEntry.Text, weather.Title, weather.Icon);
					MessagingCenter.Send(HistoryRecorder.Instance, HistoryRecorder.LocationSubmitted, item);
				}
			}
		}

	    private async void GetWeatherButton_Click(object sender, RoutedEventArgs e)
	    {
		    await GetWeather();
	    }
    }
}
