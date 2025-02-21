namespace SankoHospital.MvcWebUI.Models.NurseModel;

public class PatientViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string? BloodPressure { get; set; }   // Örneğin "120/80"
    public int? Pulse { get; set; }              // Nabız sayısı
    public string? BloodSugar { get; set; }      // Kan şekeri (format tercihinize göre)
    public DateTime AdmissionDate { get; set; }
    public DateTime? CheckoutDate { get; set; }
    public bool Checked { get; set; }
    public int RoomId { get; set; }
    public int? RoomNumber { get; set; }  // Display purposes
}