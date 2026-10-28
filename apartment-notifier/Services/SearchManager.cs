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
            BrowserService browser = new BrowserService();
            await browser.InitAsync();

            List<string> places = new List<string>
            {
                "Åstorp",
                "Helsingborg",
                "Göteborg"
            };

            BalderSource balderSource = new BalderSource(browser, places, 3, 10);

            var test = await balderSource.GetApartmentsAsync();
        }
    }
}
