namespace SankoHospital.MvcWebUI.Models.CleanerModel;

public class CleanerDashboardViewModel
{
    public int TodaysCleaningTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int AssignedAreas { get; set; }
}
