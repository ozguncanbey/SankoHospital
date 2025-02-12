namespace SankoHospital.MvcWebUI.Models.ReceptionistModel;

public class ReceptionistDashboardViewModel
{
    public int TodaysAppointments { get; set; }
    public int CheckIns { get; set; }
    public int WaitingPatients { get; set; }
    public int TotalPatients { get; set; }
}
