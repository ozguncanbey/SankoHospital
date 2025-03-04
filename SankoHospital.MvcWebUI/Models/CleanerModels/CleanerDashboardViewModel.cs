namespace SankoHospital.MvcWebUI.Models.CleanerModels;

public class CleanerDashboardViewModel
{
    // Bugün yapılması gereken temizlik görevleri
    public int TodaysRoomCleaningTasks { get; set; }
    public int TodaysBedCleaningTasks { get; set; }

    // Tamamlanan görevler
    public int CompletedRoomTasks { get; set; }
    public int CompletedBedTasks { get; set; }

    // Bekleyen görevler
    public int PendingRoomTasks { get; set; }
    public int PendingBedTasks { get; set; }
}