using ApartmentNotifier.Models;

namespace ApartmentNotifier.Sources
{
    public interface IApartmentSource
    {
        /// <summary>
        /// Fetches a list of apartment objects from the source asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Apartment>> GetApartmentsAsync();
    }
}
