using ApartmentNotifier.Models;
using ApartmentNotifier.Sources;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace ApartmentNotifier.Services
{
    public class SearchManager
    {
        public async Task RunAsync()
        {
            await using var browser = new BrowserService();
            await browser.InitAsync();

            //Will later need to be some sort of config file or database to store sources and places to search for
            List<string> cities = new List<string>
            {
                "Åstorp",
                //"Helsingborg",
                //"Göteborg"
            };

            var allApartments = new List<Apartment>();

            var sources = new List<IApartmentSource>
            {
                new BalderSource(browser, cities, 3, 10)

                //Examples for future implementation:
                //new BostadsförmedlingenSource(),
                //new HSBSource(),  
                //new RiksbyggenSource()
            };

            foreach(var source in sources)
            {
                var apartments = await source.GetApartmentsAsync();
                allApartments.AddRange(apartments);
            }

            foreach(var apartment in allApartments)
            {
                Console.WriteLine(apartment.ToString());
            }

            //TODO: Add logic for sorting through already seen apartments and notifying user of new ones

        }
    }
}
