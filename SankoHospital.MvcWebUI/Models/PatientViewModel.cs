namespace SankoHospital.MvcWebUI.Models;

public class PatientViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public DateTime? CheckoutDate { get; set; }
    public bool Checked { get; set; }
    public int RoomId { get; set; }
    public string? RoomNumber { get; set; }  // Display purposes
}