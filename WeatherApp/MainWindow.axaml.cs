using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnGetWeatherClicked(object sender, RoutedEventArgs e)
    {
        string cityName = CityInput.Text;
        if (!string.IsNullOrWhiteSpace(cityName))
        {
            await GetWeatherDataAsync(cityName);
        }
        else
        {
            WeatherInfo.Text = "Veuillez entrer une ville.";
        }
    }

    private void OnViewForecastClicked(object sender, RoutedEventArgs e)
    {
        string cityName = CityInput.Text;
        if (!string.IsNullOrWhiteSpace(cityName))
        {
            var forecastWindow = new ForecastWindow(cityName);
            forecastWindow.Show();
        }
        else
        {
            WeatherInfo.Text = "Veuillez entrer une ville.";
        }
    }


    private async Task GetWeatherDataAsync(string cityName)
    {
        string apiKey = "88e6af6ceb39e790a033c5c050aa685d";
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse);

                WeatherInfo.Text = $"Ville : {weatherData.Name}\n" +
                                   $"Latitude : {weatherData.Coord.Lat}\n" +
                                   $"Longitude : {weatherData.Coord.Lon}\n" +
                                   $"Température : {weatherData.Main.Temp}°C\n" +
                                   $"Humidité : {weatherData.Main.Humidity}%\n" +
                                   $"Description : {weatherData.Weather[0].Description}";

                string iconUrl = $"https://openweathermap.org/img/wn/{weatherData.Weather[0].Icon}@2x.png";
                await LoadWeatherIconAsync(iconUrl);
            }
        }
        catch
        {
            WeatherInfo.Text = "Erreur lors de la récupération des données. Vérifiez la ville ou la connexion.";
        }
    }

    private async Task LoadWeatherIconAsync(string iconUrl)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(iconUrl);
                response.EnsureSuccessStatusCode();
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    WeatherIcon.Source = new Bitmap(stream);
                }
            }
        }
        catch
        {
            WeatherInfo.Text += "\nErreur lors du chargement de l'icône.";
        }
    }
}

public class WeatherData
{
    public string Name { get; set; }
    public Coord Coord { get; set; }
    public Main Main { get; set; }
    public Weather[] Weather { get; set; }
}

public class Coord
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class Main
{
    public double Temp { get; set; }
    public int Humidity { get; set; }
}

public class Weather
{
    public string Description { get; set; }
    public string Icon { get; set; }
}
