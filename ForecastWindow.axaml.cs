using Avalonia.Controls;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System;  

namespace WeatherApp
{
    public partial class ForecastWindow : Window
    {
        public ForecastWindow(string cityName)
        {
            InitializeComponent();
            LoadForecast(cityName);
        }

        private async void LoadForecast(string cityName)
        {
            string apiKey = "88e6af6ceb39e790a033c5c050aa685d";
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={apiKey}&units=metric";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    ForecastData forecastData = JsonConvert.DeserializeObject<ForecastData>(jsonResponse);

                    var dailyForecasts = new List<ListItem>();
                    foreach (var item in forecastData.List)
                    {
                        if (item.DtTxt.Contains("12:00:00"))
                        {
                            dailyForecasts.Add(new ListItem
                            {
                                City = forecastData.City.Name,
                                Latitude = forecastData.City.Coord.Lat.ToString("F2"),
                                Longitude = forecastData.City.Coord.Lon.ToString("F2"),
                                Date = item.DtTxt.Split(' ')[0],
                                Time = "12:00",
                                Temperature = $"{item.Main.Temp}°C",
                                Description = item.Weather[0].Description,
                                Humidity = $"{item.Main.Humidity}%",
                                IconUrl = $"https://openweathermap.org/img/wn/{item.Weather[0].Icon}@2x.png"
                            });
                        }
                    }

                    ForecastList.ItemsSource = dailyForecasts;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur: {ex.Message}");
                ForecastList.ItemsSource = new[] { new ListItem { Description = "Erreur lors de la récupération des prévisions." } };
            }
        }
    }

    public class ForecastData
    {
        public City City { get; set; }
        public List<ForecastItem> List { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
        public Coord Coord { get; set; }
    }

    public class ForecastItem
    {
        public Main Main { get; set; }
        public List<Weather> Weather { get; set; }
        public string DtTxt { get; set; }
    }

    public class ListItem
    {
        public string City { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Temperature { get; set; }
        public string Description { get; set; }
        public string Humidity { get; set; }
        public string IconUrl { get; set; }
    }
}
