using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            var rooms = from room in _context.Rooms
                        where room.HotelId == HotelId
                        select new RoomDto()
                        {
                            roomId = room.RoomId,
                            name = room.Name,
                            capacity = room.Capacity,
                            image = room.Image,
                            hotel = new HotelDto()
                            {
                                hotelId = room.HotelId,
                                name = room.Hotel.Name,
                                address = room.Hotel.Address,
                                cityId = room.Hotel.CityId,
                                cityName = room.Hotel.City.Name,
                                state = room.Hotel.City.State
                                
                            }
                        };

            return rooms.ToList();
        }

        public RoomDto AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var addedRoom = _context.Rooms
                .Include(r => r.Hotel)
                .ThenInclude(h => h.City)
                .FirstOrDefault(r => r.RoomId == room.RoomId);

            var roomDto = new RoomDto
            {
                roomId = addedRoom.RoomId,
                name = addedRoom.Name,
                capacity = addedRoom.Capacity,
                image = addedRoom.Image,
                hotel = new HotelDto
                {
                    hotelId = addedRoom.Hotel.HotelId,
                    name = addedRoom.Hotel.Name,
                    address = addedRoom.Hotel.Address,
                    cityId = addedRoom.Hotel.CityId,
                    cityName = addedRoom.Hotel.City.Name,
                    state = room.Hotel.City.State
                }
            };

            return roomDto;
        }

        public void DeleteRoom(int RoomId)
        {
            try
            {
                var roomDeleted = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
                _context.Rooms.Remove(roomDeleted);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}