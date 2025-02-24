using Moq;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class PatientDailyRecordManagerTest
{
    [Test]
    public void PatientDailyRecordManager_AddPatient_Test()
    {
        Mock<IPatientDailyRecordDal> mockDal = new Mock<IPatientDailyRecordDal>();
        //Mock<IPatientDailyRecordService> mockManager = new Mock<IPatientDailyRecordService>();
        
        
        PatientDailyRecordManager patientDailyRecordManager = new PatientDailyRecordManager(mockDal.Object);
        
        patientDailyRecordManager.Add(new PatientDailyRecord());
    }
}