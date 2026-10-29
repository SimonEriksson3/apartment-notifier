using System;
using System.Collections.Generic;
using System.Text;

namespace ApartmentNotifier.Models
{
    public class Property
    {

        public string Address { get; set; }
        public string City { get; set; }
        public double Rent { get; set; }
        public double Size { get; set; }
        public string WebAddress { get; set; }
        public string Id { get; set; }
    }
}
