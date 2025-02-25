using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IRoomOccupancyService
{
    List<RoomOccupancy> GetAll(); // Tüm hastaları getirir      
    RoomOccupancy GetById(int id); // Belirli bir hastayı getirir
    void Add(RoomOccupancy roomOccupancy); // Yeni hasta ekler           
    void Update(RoomOccupancy roomOccupancy); // Hastayı günceller          
    void Delete(RoomOccupancy roomOccupancy); // Hastayı siler
    List<RoomOccupancy> GetByRoomOccupancy(int roomId);
}