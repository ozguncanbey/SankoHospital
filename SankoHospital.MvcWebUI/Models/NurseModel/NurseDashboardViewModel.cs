namespace SankoHospital.MvcWebUI.Models.NurseModel;

public class NurseDashboardViewModel
{
    public int TodaysPatients { get; set; }
    public int PendingTasks { get; set; }
    public int AssignedWards { get; set; }
    public int MedicationsToAdminister { get; set; }
}
