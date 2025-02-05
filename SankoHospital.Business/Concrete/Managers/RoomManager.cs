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
}