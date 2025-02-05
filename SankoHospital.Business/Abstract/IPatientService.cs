using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IPatientService
{
    List<Patient> GetAll(); // Tüm hastaları getirir      
    Patient GetById(int id);// Belirli bir hastayı getirir
    void Add(Patient patient);// Yeni hasta ekler           
    void Update(Patient patient);// Hastayı günceller          
    void Delete(Patient patient); // Hastayı siler              
}