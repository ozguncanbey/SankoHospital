namespace SankoHospital.MvcWebUI.Models.CleanerModel;

public class RoomViewModel
{
    public int Id { get; set; }
    public int RoomNumber { get; set; }
    public int Capacity { get; set; }
    public int CurrentPatientCount { get; set; }
    public string Status { get; set; }
    public DateTime? LastCleanedDate { get; set; }
}