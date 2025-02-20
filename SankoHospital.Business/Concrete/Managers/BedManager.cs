using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class BedManager : IBedService
{
    private IBedDal _bedDal;

    public BedManager(IBedDal bedDal)
    {
        _bedDal = bedDal;
    }

    public List<Bed> GetAll()
    {
        return _bedDal.GetAll();
    }

    public Bed GetById(int id)
    {
        return _bedDal.GetById(id);
    }

    public void Add(Bed patient)
    {
        _bedDal.Add(patient);
    }

    public void Update(Bed patient)
    {
        _bedDal.Update(patient);
    }

    public void Delete(Bed patient)
    {
        _bedDal.Delete(patient);
    }
}