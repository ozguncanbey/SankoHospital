using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IBedOccupancyService
{
    List<BedOccupancy> GetAll(); // Tüm hastaları getirir      
    BedOccupancy GetById(int id); // Belirli bir hastayı getirir
    void Add(BedOccupancy bedOccupancy); // Yeni hasta ekler           
    void Update(BedOccupancy bedOccupancy); // Hastayı günceller          
    void Delete(BedOccupancy bedOccupancy); // Hastayı siler
    List<BedOccupancy> GetByBedOccupancy(int bedId);
    BedOccupancy? GetOpenRecordByPatientId(int patientId);
}