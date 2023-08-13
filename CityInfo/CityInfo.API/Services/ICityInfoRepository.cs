using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetAllCitiesAsync();
        Task<City?> GetCityByIdAsync(int cityId, bool includePointOfInterest);
        Task<bool> CityExistsAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);
        Task<PointOfInterest?> GetPointOfInterestIfExistsAsync(int cityId, int pointOfInterestId);
        Task<IEnumerable<PointOfInterest>> GetAllPointOfInterestsAsync(int cityId);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterestForCityAsync(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
