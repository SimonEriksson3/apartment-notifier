namespace ApartmentNotifier.Models
{
    public class Apartment : Property
    {

        public Apartment(string address, double rent, double size, double bedrooms, string webAddress, string id)
        {
            Address = address;
            Rent = rent;
            Size = size;
            Bedrooms = bedrooms;
            WebAddress = webAddress;
            Id = id;
        }
    }
}
