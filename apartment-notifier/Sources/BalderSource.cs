using ApartmentNotifier.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApartmentNotifier.Sources
{
    internal class BalderSource : IApartmentSource
    {
        public Task<IEnumerable<Apartment>> GetApartmentsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
