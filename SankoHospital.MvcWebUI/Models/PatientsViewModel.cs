namespace SankoHospital.MvcWebUI.Models;

public class PatientsViewModel
{
    public List<PatientViewModel> Patients { get; set; } = new();
    public List<RoomViewModel> AvailableRooms { get; set; } = new();
}