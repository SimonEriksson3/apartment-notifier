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
        private readonly List<IApartmentSource> _sources = new List<IApartmentSource>();

        public SearchManager()
        {

        }

        public async Task RunAsync()
        {
            //Loading search criteria for BalderSource
            var balderCriteria = await SearchCriteria.LoadForSourceAsync("Balder");

            await using var browser = new BrowserService();
            await browser.InitAsync();

            var allApartments = new List<Apartment>();

            _sources.Add(new BalderSource(browser, balderCriteria));

            foreach (var source in _sources)
            {   
                var apartments = await source.GetApartmentsAsync();
                var filteredApartments = FilterApartments(apartments, source.Criteria);
                allApartments.AddRange(filteredApartments);
            }

            foreach(var apartment in allApartments)
            {
                Console.WriteLine(apartment.ToString());
            }

            //TODO: Add logic for sorting through already seen apartments and notifying user of new ones

        }

        public IEnumerable<Apartment> FilterApartments(IEnumerable<Apartment> apartments, SearchCriteria criteria) {

            var filtered = apartments.AsEnumerable();

            if (criteria.MaxRent.HasValue)
            {
                filtered = filtered.Where(a => a.Rent > 0 && a.Rent <= criteria.MaxRent.Value);
            }

            if (criteria.MinSize.HasValue)
            {
                filtered = filtered.Where(a => a.Size >= criteria.MinSize.Value);
            }

            if (criteria.MinBedrooms.HasValue)
            {
                filtered = filtered.Where(a => a.Bedrooms >= criteria.MinBedrooms.Value);
            }

            if (criteria.MaxBedrooms.HasValue)
            {
                filtered = filtered.Where(a => a.Bedrooms <= criteria.MaxBedrooms.Value);
            }

            return filtered.ToList();
        }
    }
}
