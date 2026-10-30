using ApartmentNotifier.Models;
using ApartmentNotifier.Services;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApartmentNotifier.Sources
{
    /// <summary>
    /// Balder is a residential landlord in Sweden and has a website with available apartments. 
    /// Used as an example of how to implement a source for the apartment notifier using Playwright as a dependency.
    /// </summary>
    public class BalderSource : IApartmentSource
    {
        public SearchCriteria Criteria { get; }
        private BrowserService _browser;
        private IPage _page;
        private string _url;

        public BalderSource(BrowserService browser, SearchCriteria criteria)
        {
            Criteria = criteria;

            string baseUrl = "https://www.balder.se/lediga-lagenheter/";
            _browser = browser;
            //We apply some of the search criterias now to filter out some of the apartments directly. 
            //Add number of rooms to the URL

            if (Criteria.MinBedrooms.HasValue)
            {
                _url = $"{baseUrl}?range[numberOfRooms]={criteria.MinBedrooms}%3A";
            }
            else
            {
                _url = baseUrl;
            }
        }

        public async Task<IEnumerable<Apartment>> GetApartmentsAsync()
        {
            //Initial test, needs work

            Console.WriteLine($"Getting apartments from Balder for locations: {string.Join(", ", Criteria.Locations)}");

            var apartments = new List<Apartment>();
            bool hasMorePages = true;

            try
            {
                //Initiate browser tab 
                if (_page == null)
                {
                    _page = await _browser.CreatePageAsync();
                }

                //Go to website 
                await _browser.GoToAsync(_page, _url);
                Console.WriteLine("Declining cookies...");
                await _browser.DeclineCookies(_page, "Avvisa");

                //Apply location filter
                await ApplyLocationsFilterAsync(_page);

                //Loop through the pages until there are no more apartments to load
                while (hasMorePages)
                {
                    var apartmentIdentifiers = await _page.Locator("article").AllAsync();

                    if (apartmentIdentifiers.Count == 0)
                    {
                        hasMorePages = false;
                        break;
                    }

                    foreach (var identifier in apartmentIdentifiers)
                    {
                        var apartment = await ParseApartmentIdentifiersAsync(identifier);
                        if (apartment != null)
                        {
                            apartments.Add(apartment);
                        }
                    }

                    hasMorePages = await TryGoToNextPageAsync(_page);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting apartments from Balder: {ex.Message}");
            }
            finally
            {
                if (_page != null)
                {
                    //Close the page after use
                    await _page.CloseAsync();
                }
            }

            return apartments;
        }

        private async Task ApplyLocationsFilterAsync(IPage page)
        {
            //Apply location filter
            var searchField = page.GetByPlaceholder("Sök på kommun", new() { Exact = false });
            await searchField.WaitForAsync(new() { State = WaitForSelectorState.Visible });

            //Add each city to the search field and apply
            foreach (string location in Criteria.Locations)
            {
                await searchField.FillAsync(location);
                var locationOption = page.GetByRole(AriaRole.Option, new() { Name = location }).First;
                await locationOption.ClickAsync();
                await page.WaitForTimeoutAsync(200);
            }
        }

        private async Task<Apartment> ParseApartmentIdentifiersAsync(ILocator identifier)
        {
            var linkElement = identifier.Locator("xpath=ancestor::a");

            if (await linkElement.CountAsync() == 0) {
                return null;
            }

            string href = await linkElement.GetAttributeAsync("href") ?? "Unknown";

            if (href.Contains("/projekt/") || string.IsNullOrEmpty(href)) return null; //Skip project links

            //Apartment address and location
            var address = (await identifier.Locator("h3").TextContentAsync())?.Trim() ?? "Unknown address";
            var location = (await identifier.Locator("h3 + span").TextContentAsync())?.Trim(); //Find the span that follows the h3 element

            string city = location ?? "Unknown city";
            string area = "";

            if (!string.IsNullOrEmpty(location) && location.Contains('–'))
            {
                var locationParts = location.Split(new[] { '–' }, StringSplitOptions.RemoveEmptyEntries);
                city = locationParts[0].Trim();
                area = locationParts.LastOrDefault()?.Trim() ?? "";
            }

            var dts = await identifier.Locator("dl dt").AllTextContentsAsync();
            var dds = await identifier.Locator("dl dd").AllTextContentsAsync();

            //Extracting details from the dl dt/dd pairs
            var details = new Dictionary<string, string>();
            for (int i = 0; i < dts.Count && i < dds.Count; i++)
            {
                details[dts[i].Trim()] = dds[i].Trim();
            }

            string rentString = details.GetValueOrDefault("Hyra", "0");
            string sizeString = details.GetValueOrDefault("Storlek", "");

            return new Apartment
            {
                //Extract apartment details from the page
                Address = address,
                City = city,
                Area = area,
                Rent = ParseRent(rentString),
                Size = ParseSize(sizeString),
                Bedrooms = ParseBedrooms(sizeString),
                WebAddress = new Uri(new Uri("https://www.balder.se"), href).ToString(),
                Id = href.Split('/').LastOrDefault() ?? ""
            };
        }

        private async Task<bool> TryGoToNextPageAsync(IPage page)
        {
            var nextPageButton = page.GetByRole(AriaRole.Link, new() { Name = "Nästa sida" });
            if (await nextPageButton.CountAsync() > 0 && await nextPageButton.IsVisibleAsync())
            {
                Console.WriteLine("Going to next page...");
                await nextPageButton.ClickAsync();
                await page.WaitForTimeoutAsync(1000);
                return true;
            }

            Console.WriteLine("No more pages to load.");
            return false;
        }

        private double ParseRent(string rentText)
        {
            var digitsOnly = new string(rentText.Where(char.IsDigit).ToArray());
            return double.TryParse(digitsOnly, out double rent) ? rent : 0;
        }

        private double ParseSize(string sizeText)
        {
            var parts = sizeText.Split('|')[0];

            // Hittar det första talet (även med decimaler om det finns)
            var match = System.Text.RegularExpressions.Regex.Match(parts, @"\d+([.,]\d+)?");

            if (match.Success && double.TryParse(match.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture, out double size))
            {
                return size;
            }

            return 0;
        }
        private double ParseBedrooms(string bedroomsText)
        {
            var parts = bedroomsText.Split('|');
            if (parts.Length < 2) return 0;

            var roomParts = parts[1].Trim().Split(' ');
            if (roomParts.Length == 0) return 0;

            string rawNumber = roomParts[0].Replace(',', '.');

            return double.TryParse(rawNumber, System.Globalization.CultureInfo.InvariantCulture, out double bedrooms) ? bedrooms : 0;
        }
    }
}
