namespace SankoHospital.MvcWebUI.Models.ReceptionistModel;

public class ReceptionistDashboardViewModel
{
    public int TodaysAdmissions { get; set; }
    public int TodaysCheckouts { get; set; }
    public int RoomsAvailable { get; set; }
    public int TotalRegisteredPatients { get; set; }
}