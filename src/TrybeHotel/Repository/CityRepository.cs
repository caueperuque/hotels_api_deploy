using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            IEnumerable<CityDto> Cities = from city in _context.Cities
                                          select new CityDto() { cityId = city.CityId, name = city.Name, state = city.State };

            return Cities.ToList();
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            try
            {
                _context.Cities.Add(city);
                _context.SaveChanges();
                var query = from reqCity in _context.Cities
                            where reqCity.CityId == city.CityId
                            select new CityDto() { cityId = reqCity.CityId, name = city.Name, state = city.State };

                return query.First();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new CityDto();
            }
        }

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
           _context.Cities.Update(city);
           _context.SaveChanges();

           return new CityDto
           {
            cityId = city.CityId,
            state = city.State,
            name = city.Name
           };
        }

    }
}