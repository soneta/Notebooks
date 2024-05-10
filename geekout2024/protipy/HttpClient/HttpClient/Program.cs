namespace HttpClient
{
    using System.Net.Http;

    internal class Program
    {
        private static readonly int connections = 20;
        private static readonly HttpClient httpClient = new HttpClient();

        private static async Task Main()
        {
            await HttpClientWithUsingAsync();
            await HttpClientWithStaticInstanceAsync();
        }

        private static async Task HttpClientWithUsingAsync()
        {
            try
            {
                Console.WriteLine("Starting connections - using");

                for (var i = 0; i <= connections; i++)
                {
                    using (var httpClient = new HttpClient())
                    {
                        var result = await httpClient.GetAsync("https://www.enova.pl/");
                        Console.WriteLine(result.StatusCode);
                    }
                }

                Console.WriteLine("Connections done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static async Task HttpClientWithStaticInstanceAsync()
        {
            try
            {
                Console.WriteLine("Starting connections - static instance");

                for (var i = 0; i <= connections; i++)
                {
                    var result = await httpClient.GetAsync("https://www.enova.pl/");
                    Console.WriteLine(result.StatusCode);
                }

                Console.WriteLine("Connections done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
