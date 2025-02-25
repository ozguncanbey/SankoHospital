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
        return _patientDailyRecordDal.GetAll().Where(p => p.PatientId == patientId).ToList();
    }
}