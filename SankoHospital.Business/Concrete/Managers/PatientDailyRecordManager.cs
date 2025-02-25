using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class PatientDailyRecordManager : IPatientDailyRecordService
{
    private readonly IPatientDailyRecordDal _patientDailyRecordDal;

    public PatientDailyRecordManager(IPatientDailyRecordDal patientDailyRecordDal)
    {
        _patientDailyRecordDal = patientDailyRecordDal;
    }

    public List<PatientDailyRecord> GetAll()
    {
        return _patientDailyRecordDal.GetAll();
    }

    public PatientDailyRecord GetById(int id)
    {
        return _patientDailyRecordDal.GetById(id);
    }

    public void Add(PatientDailyRecord patientDailyRecord)
    {
        _patientDailyRecordDal.Add(patientDailyRecord);
    }

    public void Update(PatientDailyRecord patientDailyRecord)
    {
        _patientDailyRecordDal.Update(patientDailyRecord);
    }

    public void Delete(PatientDailyRecord patientDailyRecord)
    {
        _patientDailyRecordDal.Delete(patientDailyRecord);
    }

    public List<PatientDailyRecord> GetByPatientDailyRecords(int patientId)
    {
        return _patientDailyRecordDal.GetAll().Where(p => p.PatientId == patientId).OrderByDescending(p=>p.RecordDate).ToList();
    }
    
    public List<PatientDailyRecord> GetFilteredPatientDailyRecordService(
        int? id,
        int? patientId,
        string? bloodPressure,
        string? pulse,
        string? bloodSugar,
        DateTime? recordDate)
    {
        // Tüm kayıtları çekelim (örneğin, _patientDailyRecordDal.GetAll() metodu)
        var records = _patientDailyRecordDal.GetAll();

        if (id.HasValue)
        {
            records = records.Where(r => r.Id == id.Value).ToList();
        }

        if (patientId.HasValue)
        {
            records = records.Where(r => r.PatientId == patientId.Value).ToList();
        }

        if (!string.IsNullOrEmpty(bloodPressure))
        {
            records = records.Where(r => 
                !string.IsNullOrEmpty(r.BloodPressure) && 
                r.BloodPressure.Contains(bloodPressure, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(pulse))
        {
            // Eğer pulse parametresi string olarak geliyorsa, sayı formatına çevirebilir veya basitçe string araması yapabilirsiniz.
            records = records.Where(r => 
                r.Pulse.HasValue && 
                r.Pulse.Value.ToString().Contains(pulse)).ToList();
        }

        if (!string.IsNullOrEmpty(bloodSugar))
        {
            records = records.Where(r => 
                !string.IsNullOrEmpty(r.BloodSugar) && 
                r.BloodSugar.Contains(bloodSugar, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (recordDate.HasValue)
        {
            records = records.Where(r => r.RecordDate.Date == recordDate.Value.Date).ToList();
        }

        return records;
    }

}