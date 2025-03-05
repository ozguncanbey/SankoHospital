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
        var roomsQuery = _roomDal.GetAll().AsQueryable();

        if (id.HasValue)
            roomsQuery = roomsQuery.Where(r => r.Id == id.Value);

        if (roomNumber.HasValue && roomNumber.Value > 0)
            roomsQuery = roomsQuery.Where(r => r.RoomNumber == roomNumber.Value);

        if (capacity.HasValue && capacity.Value > 0)
            roomsQuery = roomsQuery.Where(r => r.Capacity == capacity.Value);

        if (currentPatientCount.HasValue && currentPatientCount.Value >= 0)
            roomsQuery = roomsQuery.Where(r => r.CurrentPatientCount == currentPatientCount.Value);

        if (!string.IsNullOrEmpty(status))
            roomsQuery = roomsQuery.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (lastCleanedDate.HasValue)
            roomsQuery = roomsQuery.Where(r =>
                r.LastCleanedDate.HasValue && r.LastCleanedDate.Value.Date == lastCleanedDate.Value.Date);

        if (!string.IsNullOrEmpty(occupancy))
        {
            if (occupancy.Equals("Full", StringComparison.OrdinalIgnoreCase))
                roomsQuery = roomsQuery.Where(r => r.CurrentPatientCount >= r.Capacity);
            else if (occupancy.Equals("Empty", StringComparison.OrdinalIgnoreCase))
                roomsQuery = roomsQuery.Where(r => r.CurrentPatientCount < r.Capacity);
        }

        return roomsQuery.ToList();
    }
}