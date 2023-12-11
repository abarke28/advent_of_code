using Aoc.Common;
using System.Net;

namespace Aoc.Utils
{
    public class InputFetcher
    {
        private const string InputPath = "{0}/Day_{1}/input{2}.txt";

        private readonly AocConfig _config;

        public InputFetcher(AocConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task FetchInput(string year, string day)
        {
            var fileInput = string.Format(InputPath, year, day, string.Empty);
            var inputPath = $"C://git/aoc/{fileInput}";

            if (File.Exists(inputPath))
            {
                var currentInputFileContent = File.ReadAllLines(inputPath);

                if (currentInputFileContent.Any(l => !string.IsNullOrWhiteSpace(l)))
                {
                    Console.WriteLine($"[Input file {inputPath} already has content, will not fetch input]\n");
                    return;
                }
            }

            var baseAddress = new Uri(_config.BaseUrl);
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };
            var client = new HttpClient(handler) { BaseAddress = baseAddress };

            cookies.Add(baseAddress, new Cookie(_config.CookieName, _config.Token));

            var requestPath = string.Format(_config.RequestPath, year, int.Parse(day));

            Console.WriteLine($"[Fetching file input - GET - {baseAddress}{requestPath}]\n");

            var response = await client.GetAsync(requestPath);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching problem input: {response.StatusCode}");
                throw new Exception(response.ToString());
            }
            var content = await response.Content.ReadAsStringAsync();

            File.WriteAllText(inputPath, content);
            var lines = content.Split("\n").Where(l => !string.IsNullOrWhiteSpace(l));
        }
    }
}
