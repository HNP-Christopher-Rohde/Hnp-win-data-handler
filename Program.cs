// HNP Win Data Handler v.1.0 EXE
// Author: HNP C.R.
// Author URI: https://homepage-nach-preis.de/
// Licence: Creative Commons Non-Commercial 

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class hnp_win_data_Main
{
    private static readonly HttpClient client = new HttpClient();
    private static string domainFilePath = "hnp_win_data_domain.txt";
    private static string? domainUrl = null;

    private const string SecretKey = "9418BB768671A389";

    static async Task Main(string[] args)
    {
        client.DefaultRequestHeaders.Add("X-Secret-Key", SecretKey);

        await LoadDomain();
        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Change Domain");
            Console.WriteLine("2. Load Data from WordPress");
            Console.WriteLine("3. Change and Save Data");
            Console.Write("Select an option: ");

            string? option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    ChangeDomain();
                    break;
                case "2":
                    await LoadDataFromWordPress();
                    break;
                case "3":
                    await ChangeAndSaveData();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    static async Task LoadDomain()
    {
        if (File.Exists(domainFilePath))
        {
            domainUrl = await File.ReadAllTextAsync(domainFilePath) ?? string.Empty;
            Console.WriteLine($"Loaded domain: {domainUrl}");
        }
        else
        {
            Console.Write("Enter domain URL: ");
            domainUrl = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(domainUrl))
            {
                await File.WriteAllTextAsync(domainFilePath, domainUrl);
            }
        }
    }

    static void ChangeDomain()
    {
        Console.Write("Enter new domain URL: ");
        string? newDomainUrl = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDomainUrl))
        {
            domainUrl = newDomainUrl;
            File.WriteAllText(domainFilePath, domainUrl);
            Console.WriteLine("Domain updated successfully.");
        }
        else
        {
            Console.WriteLine("Invalid domain URL provided.");
        }
    }

    static async Task LoadDataFromWordPress()
    {
        if (!string.IsNullOrWhiteSpace(domainUrl))
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(domainUrl + "/wp-json/hnp-win-data/v1/data");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                string decodedData = JsonConvert.DeserializeObject<string>(responseBody) ?? "Default or empty data";
                Console.WriteLine("Data loaded: " + decodedData);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
        else
        {
            Console.WriteLine("Domain URL is not set. Please set the domain URL first.");
        }
    }

    static async Task ChangeAndSaveData()
    {
        Console.Write("Enter new data value: ");
        string? newData = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newData) && !string.IsNullOrWhiteSpace(domainUrl))
        {
            var content = new StringContent($"\"{newData}\"", System.Text.Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await client.PostAsync(domainUrl + "/wp-json/hnp-win-data/v1/data", content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Data saved successfully.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
        else
        {
            Console.WriteLine("Invalid data value or domain URL is not set.");
        }
    }
}
