using System;
using System.Collections.Generic;
using System.Text;

namespace ApartmentNotifier.Models
{
    public class Property
    {

        protected string Address { get; set; }
        protected double Rent { get; set; }
        protected double Size { get; set; }
        protected double Bedrooms { get; set; }
        protected string WebAddress { get; set; }
        protected string Id { get; set; }
    }
}
