namespace SankoHospital.MvcWebUI.Models.NurseModel;

public class PatientRecordsViewModel
{
    public PatientRecordsViewModel()
    {
        Records = new List<RecordsViewModel>();
    }

    // Patient's basic information
    public PatientViewModel PatientInfo { get; set; } = new PatientViewModel();

    // List of daily records for the patient
    public List<RecordsViewModel> Records { get; set; }
}