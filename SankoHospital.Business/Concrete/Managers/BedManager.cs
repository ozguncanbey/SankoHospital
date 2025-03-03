using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class BedManager : IBedService
{
    private readonly IBedDal _bedDal;
    private readonly IBedOccupancyService _bedOccupancyManager;
    private readonly IRoomService _roomManager;

    public BedManager(IBedDal bedDal, IRoomService roomManager, IBedOccupancyService bedOccupancyManager)
    {
        _bedDal = bedDal;
        _roomManager = roomManager;
        _bedOccupancyManager = bedOccupancyManager;
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
        int? roomNumber, // Oda numarası (RoomNumber)
        int? bedNumber,
        int? patientId,
        string status,
        DateTime? lastCleanedDate)
    {
        // Tüm yatakları alalım:
        var beds = _bedDal.GetAll();

        // ID filtrelemesi
        if (id.HasValue)
        {
            beds = beds.Where(b => b.Id == id.Value).ToList();
        }

        // Oda numarası filtrelemesi: 
        // Burada, _roomManager.GetById(b.RoomId)?.RoomNumber ile oda numarası çekiliyor.
        if (roomNumber.HasValue && roomNumber.Value > 0)
        {
            beds = beds.Where(b => _roomManager.GetById(b.RoomId)?.RoomNumber == roomNumber.Value).ToList();
        }

        // Yatak numarası filtrelemesi
        if (bedNumber.HasValue && bedNumber.Value > 0)
        {
            beds = beds.Where(b => b.BedNumber == bedNumber.Value).ToList();
        }

        // Hasta ID filtrelemesi
        if (patientId.HasValue && patientId.Value > 0)
        {
            beds = beds.Where(b => b.PatientId.HasValue && b.PatientId.Value == patientId.Value).ToList();
        }

        // Durum filtrelemesi
        if (!string.IsNullOrEmpty(status))
        {
            beds = beds.Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Son Temizlik Tarihi filtrelemesi (tarih kısmı eşleşsin)
        if (lastCleanedDate.HasValue)
        {
            beds = beds.Where(b => b.LastCleanedDate.HasValue &&
                                   b.LastCleanedDate.Value.Date == lastCleanedDate.Value.Date).ToList();
        }

        return beds;
    }
}