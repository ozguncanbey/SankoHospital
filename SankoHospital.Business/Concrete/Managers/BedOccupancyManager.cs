using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class BedOccupancyManager : IBedOccupancyService
{
    private readonly IBedOccupancyDal _bedOccupancyDal;

    public BedOccupancyManager(IBedOccupancyDal bedOccupancyDal)
    {
        _bedOccupancyDal = bedOccupancyDal;
    }

    public List<BedOccupancy> GetAll()
    {
        return _bedOccupancyDal.GetAll();
    }

    public BedOccupancy GetById(int id)
    {
        return _bedOccupancyDal.GetById(id);
    }

    public void Add(BedOccupancy bedOccupancy)
    {
        _bedOccupancyDal.Add(bedOccupancy);
    }

    public void Update(BedOccupancy bedOccupancy)
    {
        _bedOccupancyDal.Update(bedOccupancy);
    }

    public void Delete(BedOccupancy bedOccupancy)
    {
        _bedOccupancyDal.Delete(bedOccupancy);
    }

    public List<BedOccupancy> GetByBedOccupancy(int bedId)
    {
        return _bedOccupancyDal.GetAll().Where(b => b.BedId == bedId).ToList();
    }

    public BedOccupancy? GetOpenRecordByPatientId(int patientId)
    {
        return _bedOccupancyDal.GetAll()
            .FirstOrDefault(b => b.PatientId == patientId && b.CheckoutDate == null);
    }
}