namespace SankoHospital.MvcWebUI.Models.NurseModel;

public class RecordsViewModel
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string? BloodPressure { get; set; }   // Örneğin "120/80"
    public int? Pulse { get; set; }              // Nabız sayısı
    public string? BloodSugar { get; set; }      // Kan şekeri (format tercihinize göre)
    public DateTime RecordDate { get; set; }
}