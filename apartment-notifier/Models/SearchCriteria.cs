using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ApartmentNotifier.Models
{
    public class SearchCriteria
    {
        public List<string> Locations { get; set; } = new();
        public double? MaxRent { get; set; }
        public double? MinSize { get; set; }
        public double? MinBedrooms { get; set; }
        public double? MaxBedrooms { get; set; }

        public SearchCriteria()
        {

        }

        public SearchCriteria(List<string> locations, double? maxRent = null, double? minSize = null, double? minBedrooms = null, double? maxBedrooms = null)
        {
            Locations = locations;
            MaxRent = maxRent;
            MinSize = minSize;
            MinBedrooms = minBedrooms;
            MaxBedrooms = maxBedrooms;
        }

        //Load search criteria for a specific source from appsettings.json
        public static async Task<SearchCriteria> LoadForSourceAsync(string sourceName, string fileName = "appsettings.json")
        {

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist. Using empty search criteria.");
                return new SearchCriteria();
            }

            string jsonText = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(jsonText);

            //Search for sources
            if (doc.RootElement.TryGetProperty("Sources", out var sources) &&
                sources.TryGetProperty(sourceName, out var sourceElement))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                return JsonSerializer.Deserialize<SearchCriteria>(sourceElement.GetRawText(), options) ?? new SearchCriteria();
            }

            Console.WriteLine($"Source {sourceName} not found in {filePath}. Using empty criteria.");
            return new SearchCriteria();
        }
    }
}
