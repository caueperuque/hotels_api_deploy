using System.Net.Http;
using System.Net.Http.Headers;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "aspnet-user-agent");
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var response = await _client.GetAsync("https://nominatim.openstreetmap.org/status.php?format=json");
            if (response is null) return default(Object);

            var result = await response.Content.ReadFromJsonAsync<object>();
            return result;
        }

        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var address = $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1";
            var response = await _client.GetAsync(address);

            if (!response.IsSuccessStatusCode) return default(GeoDtoResponse);

            var result = await response.Content.ReadFromJsonAsync<List<GeoDtoResponse>>();
            return new GeoDtoResponse
            {
                lat = result[0].lat,
                lon = result[0].lon
            };
        }

        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var userCoordinates = await GetGeoLocation(geoDto);
            var hotels = repository.GetHotels();
            var hotelsWithDistances = hotels.Select(async hotel =>
            {
                var hotelAddress = new GeoDto
                {
                    Address = hotel.address,
                    City = hotel.cityName,
                    State = hotel.state,
                };
                
                var hotelCoordinates = await GetGeoLocation(hotelAddress);
                var hotelDistance = CalculateDistance(userCoordinates.lat, userCoordinates.lon, hotelCoordinates.lat, hotelCoordinates.lon);

                return new GeoDtoHotelResponse
                {
                    HotelId = hotel.hotelId,
                    Name = hotel.name,
                    Address = hotel.address,
                    CityName = hotel.cityName,
                    State = hotel.state,
                    Distance = hotelDistance,
                };
            }).ToList();

            var hotelsDistance = await Task.WhenAll(hotelsWithDistances);

            return hotelsDistance.OrderBy(hotel => hotel.Distance).ToList();
        }

        public int CalculateDistance(string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny)
        {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.', ','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.', ','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.', ','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.', ','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return int.Parse(Math.Round(distance, 0).ToString());
        }

        public double radiano(double degree)
        {
            return degree * Math.PI / 180;
        }

    }
}