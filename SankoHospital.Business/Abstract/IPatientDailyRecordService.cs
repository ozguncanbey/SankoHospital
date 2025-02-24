using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IPatientDailyRecordService
{
    List<PatientDailyRecord> GetAll(); // Tüm hastaları getirir      
    PatientDailyRecord GetById(int id);// Belirli bir hastayı getirir
    void Add(PatientDailyRecord patientDailyRecord);// Yeni hasta ekler           
    void Update(PatientDailyRecord patientDailyRecord);// Hastayı günceller          
    void Delete(PatientDailyRecord patientDailyRecord); // Hastayı siler     
    List<PatientDailyRecord> GetFilteredPatientDailyRecordService(
        int? id,
        int? patientId,
        string? bloodPressure,
        string? pulse,
        string? bloodSugar,
        DateTime? recordDate);
}