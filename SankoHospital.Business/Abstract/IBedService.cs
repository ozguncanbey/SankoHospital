using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IBedService
{
    List<Bed> GetAll(); // Tüm hastaları getirir      
    Bed GetById(int id);// Belirli bir hastayı getirir
    void Add(Bed patient);// Yeni hasta ekler           
    void Update(Bed patient);// Hastayı günceller          
    void Delete(Bed patient); // Hastayı siler     
}