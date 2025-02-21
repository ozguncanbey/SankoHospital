using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class PatientManager : IPatientService
{
    private IPatientDal _patientDal;
    private IRoomService _roomManager;

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

    public List<Patient> GetFilteredPatients(
        int? id,
        string name,
        string surname,
        string bloodType,
        DateTime? admissionDate,
        DateTime? checkoutDate,
        int? roomId)
    {
        // Tüm hastaları alalım:
        var patients = _patientDal.GetAll();

        // ID filtrelemesi
        if (id.HasValue)
        {
            patients = patients.Where(p => p.Id == id.Value).ToList();
        }

        // Ad filtrelemesi (küçük/büyük harf duyarsız)
        if (!string.IsNullOrEmpty(name))
        {
            patients = patients.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Soyad filtrelemesi
        if (!string.IsNullOrEmpty(surname))
        {
            patients = patients.Where(p => p.Surname.Contains(surname, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Kan grubu filtrelemesi
        if (!string.IsNullOrEmpty(bloodType))
        {
            patients = patients.Where(p => p.BloodType.Equals(bloodType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Kabul tarihi filtrelemesi (tam eşleşme, dilerseniz aralık da ekleyebilirsiniz)
        if (admissionDate.HasValue)
        {
            patients = patients.Where(p => p.AdmissionDate.Date == admissionDate.Value.Date).ToList();
        }

        // Çıkış tarihi filtrelemesi
        if (checkoutDate.HasValue)
        {
            patients = patients
                .Where(p => p.CheckoutDate.HasValue && p.CheckoutDate.Value.Date == checkoutDate.Value.Date).ToList();
        }

        if (roomId.HasValue && roomId.Value > 0)
        {
            var room = _roomManager.GetById(roomId.Value);
            if (room != null)
            {
                patients = patients.Where(p => p.RoomId == roomId.Value).ToList();
            }
        }
        
        return patients;
    }
}