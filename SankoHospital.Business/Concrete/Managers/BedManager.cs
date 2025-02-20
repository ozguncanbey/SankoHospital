using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class BedManager : IBedService
{
    private IBedDal _bedDal;
    private IRoomService _roomManager;

    public BedManager(IBedDal bedDal, IRoomService roomManager)
    {
        _bedDal = bedDal;
        _roomManager = roomManager;
    }

    public List<Bed> GetAll()
    {
        return _bedDal.GetAll();
    }

    public Bed GetById(int id)
    {
        return _bedDal.GetById(id);
    }

    public void Add(Bed patient)
    {
        _bedDal.Add(patient);
    }

    public void Update(Bed patient)
    {
        _bedDal.Update(patient);
    }

    public void Delete(Bed patient)
    {
        _bedDal.Delete(patient);
    }
    
    public List<Bed> GetFilteredBeds(int roomNumber, string status, string searchTerm)
    {
        // Tüm yatakları alalım:
        var beds = _bedDal.GetAll();
    
        // Oda numarası filtrelemesi:
        if (roomNumber > 0)
        {
            beds = beds.Where(b => _roomManager.GetById(b.RoomId)?.RoomNumber == roomNumber)
                .ToList();
        }
    
        // Durum filtrelemesi:
        if (!string.IsNullOrEmpty(status))
        {
            beds = beds.Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Arama filtrelemesi:
        if (!string.IsNullOrEmpty(searchTerm))
        {
            beds = beds.Where(b =>
                b.RoomId.ToString().Contains(searchTerm) ||
                b.BedNumber.ToString().Contains(searchTerm) ||
                (b.PatientId.HasValue && b.PatientId.Value.ToString().Contains(searchTerm))
            ).ToList();
        }

        return beds;
    }

}