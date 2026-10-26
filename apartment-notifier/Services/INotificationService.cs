using ApartmentNotifier.Models;

namespace ApartmentNotifier.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(IEnumerable<Apartment> apartments);
    }
}
