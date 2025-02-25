using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.DataAccess.Concrete.NHibernate;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class RoomOccupancyManager : IRoomOccupancyService
{
    private readonly IRoomOccupancyDal _roomOccupancyDal;

    public RoomOccupancyManager(IRoomOccupancyDal roomOccupancyDal)
    {
        _roomOccupancyDal = roomOccupancyDal;
    }

    public List<RoomOccupancy> GetAll()
    {
        return _roomOccupancyDal.GetAll();
    }

    public RoomOccupancy GetById(int id)
    {
        return _roomOccupancyDal.GetById(id);
    }

    public void Add(RoomOccupancy roomOccupancy)
    {
        _roomOccupancyDal.Add(roomOccupancy);
    }

    public void Update(RoomOccupancy roomOccupancy)
    {
        _roomOccupancyDal.Update(roomOccupancy);
    }

    public void Delete(RoomOccupancy roomOccupancy)
    {
        _roomOccupancyDal.Delete(roomOccupancy);
    }

    public List<RoomOccupancy> GetByRoomOccupancy(int roomId)
    {
        return _roomOccupancyDal.GetAll().Where(r => r.RoomId == roomId).ToList();
    }
}