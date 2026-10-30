using ApartmentNotifier.Models;

namespace ApartmentNotifier.Sources
{
    public interface IApartmentSource
    {
        /// <summary>
        /// Gets the search criteria used for filtering apartments from this source.
        /// </summary>
        SearchCriteria Criteria { get; }

        /// <summary>
        /// Fetches a list of apartment objects from the source asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Apartment>> GetApartmentsAsync();
    }
}
