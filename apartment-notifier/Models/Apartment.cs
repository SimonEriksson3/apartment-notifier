namespace ApartmentNotifier.Models
{
    public class Apartment : Property
    {
        public string Area { get; set; }
        public double Bedrooms { get; set; }
        public Apartment(string address, string city, string area, double rent, double size, double bedrooms, string webAddress, string id)
        {
            Address = address;
            City = city;
            Area = area;
            Rent = rent;
            Size = size;
            Bedrooms = bedrooms;
            WebAddress = webAddress;
            Id = id;
        }

        public Apartment()
        {

        }

        public string ToString()
        {
            return "Adress: " + Address + ", City: " + City + ", Area: " + Area + ", rent: " + Rent + "kr, size: " + Size + "m², bedrooms: " + Bedrooms + " RoK, web address: " + WebAddress + ", ID: " + Id;
        }
    }
}
