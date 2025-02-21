using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IRoomService
{
    List<Room> GetAll();    // Tüm odaları getirir
    Room GetById(int id);   // Belirli bir odayı getirir
    void Add(Room room);    // Yeni oda ekler
    void Update(Room room); // Odayı günceller
    void Delete(Room room); // Odayı siler

    List<Room> GetFilteredRooms(
        int? id,
        int? roomNumber,
        int? capacity,
        int? currentPatientCount,
        string status,
        DateTime? lastCleanedDate,
        string occupancy);
}