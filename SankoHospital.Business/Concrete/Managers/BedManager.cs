using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class BedManager : IBedService
{
    private readonly IBedDal _bedDal;
    private readonly IBedOccupancyService _bedOccupancyManager;
    private readonly IRoomService _roomManager;
    private readonly IPatientService _patientManager;

    public BedManager(IBedDal bedDal, IRoomService roomManager, IBedOccupancyService bedOccupancyManager,
        IPatientService patientManager)
    {
        _bedDal = bedDal;
        _roomManager = roomManager;
        _bedOccupancyManager = bedOccupancyManager;
        _patientManager = patientManager;
    }

    public List<Bed> GetAll()
    {
        return _bedDal.GetAll();
    }

    public List<Bed> GetAllByRoom(int roomId)
    {
        return _bedDal.GetAll().Where(b => b.RoomId == roomId).ToList();
    }

    public Bed GetById(int id)
    {
        return _bedDal.GetById(id);
    }

    public Bed GetByPatientId(int patientId)
    {
        return _bedDal.GetAll().FirstOrDefault(b => b.PatientId.HasValue && b.PatientId.Value == patientId);
    }

    public Bed GetByRoomId(int roomId)
    {
        return _bedDal.GetAll().FirstOrDefault(b => b.RoomId == roomId);
    }

    public Bed? GetAvailableBedForRoom(int roomId)
    {
        // Belirtilen odadaki tüm yatakları getiriyoruz
        var beds = _bedDal.GetAll().Where(b => b.RoomId == roomId);

        // Yatakları id'ye göre sıralıyoruz (örneğin, en düşük ID'li ilk tercih edilir)
        foreach (var bed in beds.OrderBy(b => b.Id))
        {
            // Bu yatak için aktif (checkout_date == null) bir BedOccupancy kaydı var mı kontrol ediyoruz
            var occupancyRecord = _bedOccupancyManager.GetByBedOccupancy(bed.Id)
                .FirstOrDefault(o => o.CheckoutDate == null);

            // Eğer böyle bir kayıt yoksa, bu yatak müsaittir
            if (occupancyRecord == null)
            {
                return bed;
            }
        }

        // Hiç müsait yatak bulunamazsa null döner
        return null;
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

    public List<Bed> GetFilteredBeds(
        int? id,
        int? roomNumber,
        int? bedNumber,
        int? patientId,
        string status,
        DateTime? lastCleanedDate)
    {
        // Tüm yatakları ve hastaları belleğe çekiyoruz
        var beds = _bedDal.GetAll().ToList();
        var patients = _patientManager.GetAll().ToDictionary(p => p.Id, p => p);

        // Filtreleme işlemini LINQ to Objects ile yapıyoruz
        var filteredBeds = beds.Where(b =>
        {
            var patient = b.PatientId.HasValue && patients.ContainsKey(b.PatientId.Value)
                ? patients[b.PatientId.Value]
                : null;
            return patient == null || !patient.CheckoutDate.HasValue;
        }).AsQueryable();

        if (id.HasValue)
        {
            filteredBeds = filteredBeds.Where(b => b.Id == id.Value);
        }

        if (roomNumber.HasValue && roomNumber.Value > 0)
        {
            filteredBeds = filteredBeds.Where(b => _roomManager.GetById(b.RoomId).RoomNumber == roomNumber.Value);
        }

        if (bedNumber.HasValue && bedNumber.Value > 0)
        {
            filteredBeds = filteredBeds.Where(b => b.BedNumber == bedNumber.Value);
        }

        if (patientId.HasValue && patientId.Value > 0)
        {
            filteredBeds = filteredBeds.Where(b => b.PatientId.HasValue && b.PatientId.Value == patientId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            filteredBeds = filteredBeds.Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (lastCleanedDate.HasValue)
        {
            filteredBeds = filteredBeds.Where(b =>
                b.LastCleanedDate.HasValue && b.LastCleanedDate.Value.Date == lastCleanedDate.Value.Date);
        }

        return filteredBeds.ToList();
    }
}