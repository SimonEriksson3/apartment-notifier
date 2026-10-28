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
        BrowserService _browser;
        IPage _page;
        List<string> _locations;
        private string _url;

        public BalderSource(BrowserService browser, List<string> locations, int minNumberOfRooms, double minSize)
        {
            string baseUrl = "https://www.balder.se/lediga-lagenheter/";

            _browser = browser;
            _locations = new List<string>(locations);
            //Add number of rooms to the URL
            _url = baseUrl += $"?range[numberOfRooms]={minNumberOfRooms}%3A";
        }

        public async Task<IEnumerable<Apartment>> GetApartmentsAsync()
        {
            //Initial test, needs work

            //Initiate browser tab 
            if (_page == null)
            {
                _page = await _browser.CreatePageAsync();
            }

            //Go to website 
            await _browser.GoToAsync(_page, _url);
            await _browser.DeclineCookies(_page, "Avvisa");

            //Apply location filter
            var searchField = _page.GetByPlaceholder("Sök på kommun", new() { Exact = false });
            await searchField.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            //Add each city to the search field and apply
            foreach (string location in _locations)
            {
                await searchField.FillAsync(location);
                var option = _page.GetByRole(AriaRole.Option, new() { Name = location }).First;
                await option.ClickAsync();
                await _page.WaitForTimeoutAsync(200);
            }

            return new List<Apartment>();
        }
    }
}
