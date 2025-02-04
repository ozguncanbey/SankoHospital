using Moq;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class PatientManagerTest
{
    [Test]
    public void Patient_validation_test()
    {
        Mock<IPatientDal> mock = new Mock<IPatientDal>();
        
        PatientManager patientManager = new PatientManager(mock.Object);
        
        patientManager.Add(new Patient());
    }
}