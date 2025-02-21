using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class RoomManager : IRoomService
{
    private IRoomDal _roomDal;

    public RoomManager(IRoomDal roomDal)
    {
        _roomDal = roomDal;
    }

    public List<Room> GetAll()
    {
        return _roomDal.GetAll();
    }

    public Room GetById(int id)
    {
        return _roomDal.GetById(id);
    }

    public void Add(Room room)
    {
        _roomDal.Add(room);
    }

    public void Update(Room room)
    {
        _roomDal.Update(room);
    }

    public void Delete(Room room)
    {
        _roomDal.Delete(room);
    }

    public List<Room> GetFilteredRooms(
        int? id,
        int? roomNumber,
        int? capacity,
        int? currentPatientCount,
        string status,
        DateTime? lastCleanedDate,
        string occupancy)
    {
        // Tüm odaları alalım:
        var rooms = _roomDal.GetAll();

        // ID filtrelemesi
        if (id.HasValue)
        {
            rooms = rooms.Where(r => r.Id == id.Value).ToList();
        }

        // Oda numarası filtrelemesi
        if (roomNumber.HasValue && roomNumber.Value > 0)
        {
            rooms = rooms.Where(r => r.RoomNumber == roomNumber.Value).ToList();
        }

        // Kapasite filtrelemesi
        if (capacity.HasValue && capacity.Value > 0)
        {
            rooms = rooms.Where(r => r.Capacity == capacity.Value).ToList();
        }

        // Mevcut hasta sayısı filtrelemesi
        if (currentPatientCount.HasValue && currentPatientCount.Value >= 0)
        {
            rooms = rooms.Where(r => r.CurrentPatientCount == currentPatientCount.Value).ToList();
        }

        // Durum filtrelemesi
        if (!string.IsNullOrEmpty(status))
        {
            rooms = rooms.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Son temizlenme tarihi filtrelemesi (sadece tarih kısmı)
        if (lastCleanedDate.HasValue)
        {
            rooms = rooms.Where(r => r.LastCleanedDate.HasValue &&
                                     r.LastCleanedDate.Value.Date == lastCleanedDate.Value.Date).ToList();
        }
        
        return rooms;
    }
}