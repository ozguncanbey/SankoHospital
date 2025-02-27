using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IBedService
{
    List<Bed> GetAll(); // Tüm hastaları getirir      
    Bed GetById(int id);// Belirli bir hastayı getirir
    Bed GetByPatientId(int patientId);
    void Add(Bed patient);// Yeni hasta ekler           
    void Update(Bed patient);// Hastayı günceller          
    void Delete(Bed patient); // Hastayı siler     

    List<Bed> GetFilteredBeds(
        int? id,
        int? roomNumber, // Oda numarası (RoomNumber)
        int? bedNumber,
        int? patientId,
        string status,
        DateTime? lastCleanedDate);
}