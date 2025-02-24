using Moq;
using SankoHospital.Business.Abstract;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class PatientManagerTest
{
    [Test]
    public void PatientManager_AddPatient_Test()
    {
        Mock<IPatientDal> mockPD = new Mock<IPatientDal>();
        Mock<IRoomService> mockRM = new Mock<IRoomService>();
        
        
        PatientManager patientManager = new PatientManager(mockPD.Object, mockRM.Object);
        
        patientManager.Add(new Patient());
    }
}