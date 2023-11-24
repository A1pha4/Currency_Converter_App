using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Currency_Converter_App
{
    class Program
    {
        static void ShowWelcomeMessage()
        {
            Console.WriteLine("\t\t\t\t\t\t\t\t\t******************************************************************");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t*                                                                *");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t*                          WELCOME TO THE ALPHA                  *");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t*                         CURRENCY CONVERTER APP                 *");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t*                                                                *");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t******************************************************************");
            Console.WriteLine();
        }
        static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Title = "Currency Converter App";

            ShowWelcomeMessage();

            string baseUrl = "https://api.exchangerate-api.com/v4/latest/USD";

            do
            {
                try
                {
                    var exchangeRates = await GetExchangeRates(baseUrl);

                    if (exchangeRates != null)
                    {
                        Console.WriteLine($"\nExchange rates for 1 {exchangeRates.Base}:");

                        foreach (var rate in exchangeRates.Rates)
                        {
                            Console.WriteLine($"{rate.Key}: {rate.Value}");
                        }

                        Console.WriteLine("\nConvert From...");
                        string fromCurrency = Console.ReadLine()?.ToUpper();

                        Console.WriteLine("\nConvert To...");
                        string toCurrency = Console.ReadLine()?.ToUpper();

                        Console.WriteLine("\nEnter the amount to convert:");
                        if (float.TryParse(Console.ReadLine(), out float amount))
                        {
                            float exchangeRate = CurrencyConverter.GetExchangeRate(fromCurrency, toCurrency, amount, exchangeRates);
                            Console.WriteLine($"From {amount} {fromCurrency} To {toCurrency} = {exchangeRate}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount entered.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Unable to fetch exchange rates.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("\nDo you want to perform another conversion? (yes/no)");
            } while (Console.ReadLine()?.ToLower() == "yes");

            Console.WriteLine("\t\t\t\t\t\t\tThank you for using the Currency Converter App.");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\tPress any key...");
            Console.ReadKey();
        }

        static async Task<ExchangeRates> GetExchangeRates(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(apiUrl);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeRates>(response);
            }
        }

        class ExchangeRates
        {
            public string Base { get; set; }
            public DateTime Date { get; set; }
            public System.Collections.Generic.Dictionary<string, float> Rates { get; set; }
        }

        class CurrencyConverter
        {
            public static float GetExchangeRate(string from, string to, float amount, ExchangeRates exchangeRates)
            {
                if (exchangeRates == null || exchangeRates.Rates == null || exchangeRates.Base == null)
                    return 0;

                if (from != null && to != null && from.ToLower() == exchangeRates.Base.ToLower() && to.ToLower() == exchangeRates.Base.ToLower())
                    return amount;

                float toRate = exchangeRates.Rates.ContainsKey(to) ? exchangeRates.Rates[to] : 0;
                float fromRate = exchangeRates.Rates.ContainsKey(from) ? exchangeRates.Rates[from] : 0;

                if (from != null && from.ToLower() == exchangeRates.Base.ToLower())
                {
                    return amount * toRate;
                }
                else if (to != null && to.ToLower() == exchangeRates.Base.ToLower())
                {
                    return amount / fromRate;
                }
                else
                {
                    return (amount * toRate) / fromRate;
                }
            }
        }
    }
}