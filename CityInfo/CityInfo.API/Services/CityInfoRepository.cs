using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        /// <summary>
        /// Retrieves a collection of cities based on the specified filters.
        /// </summary>
        /// <param name="name">Optional. The name of the city to filter by.</param>
        /// <param name="searchQuery">Optional. The search query to filter by city name or description.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the filtered collection of cities.</returns>
        public async Task<(IEnumerable<City>, PaginationMetadata)> GetAllCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a =>
                    a.Name.Contains(searchQuery) || (a.Description != null && a.Description.Contains(searchQuery)));
            }


            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection.OrderBy(c => c.Name).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        public async Task<City?> GetCityByIdAsync(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return await _context.Cities
                    .Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterest
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestIfExistsAsync(int cityId, int pointOfInterestId)
        {
            if (!await CityExistsAsync(cityId))
            {
                return null;
            }

            var pointOfInterestEntity = await GetPointOfInterestAsync(cityId, pointOfInterestId);
            return pointOfInterestEntity;
        }

        public async Task<IEnumerable<PointOfInterest>> GetAllPointOfInterestsAsync(int cityId)
        {
            return await _context.PointOfInterest
                .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityByIdAsync(cityId, false);
            city?.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterestForCityAsync(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterest.Remove(pointOfInterest);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
