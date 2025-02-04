using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class PatientManager : IPatientService
{
    private IPatientDal _patientDal;

    public PatientManager(IPatientDal patientDal)
    {
        _patientDal = patientDal;
    }

    public List<Patient> GetAll()
    {
        return _patientDal.GetAll();
    }

    public Patient GetById(int id)
    {
        return _patientDal.GetById(id);
    }

    public void Add(Patient patient)
    {
        _patientDal.Add(patient);
    }

    public void Update(Patient patient)
    {
        _patientDal.Update(patient);
    }

    public void Delete(Patient patient)
    {
        _patientDal.Delete(patient);
    }
}