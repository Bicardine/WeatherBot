using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace WeatherBot
{
    class Program
    {
        private static readonly string Token = "5508681702:AAFdrSArHaDVIAhkBw2325UzUC6W1KWI3ys";
        private static TelegramBotClient Client;
        private static string CityUserName;
        private static float CityTemperature;
        private static string CityRequestName;

        public static void Main(string[] args)
        {
            Client = new TelegramBotClient(Token) { Timeout = TimeSpan.FromSeconds(10)};

            var bot = Client.GetMeAsync().Result;
            Console.WriteLine($"Bot id: {bot.Id} \nBot name: {bot.FirstName} ");
            
            Client.OnMessage += OnMessage;
            Client.StartReceiving();
            Console.ReadLine();
            Client.StopReceiving();
        }

        private static async void OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type != MessageType.Text) return;

            CityUserName = message.Text;
            TryGetWeather(CityUserName);
            await Client.SendTextMessageAsync(message.Chat.Id, $"{CityRequestName}: {Math.Round(CityTemperature)} °C");    
           
            Console.WriteLine(message.Text);
        }

        public static void TryGetWeather(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&unit=metric&appid=2351aaee5394613fc0d14424239de2bd";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                CityRequestName = weatherResponse.Name;
                CityTemperature = weatherResponse.Main.Temp - 273;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("WebException");
                return;
            }
        }
    }
}