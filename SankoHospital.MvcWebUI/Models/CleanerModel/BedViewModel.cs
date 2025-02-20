namespace SankoHospital.MvcWebUI.Models.CleanerModel;

public class BedViewModel
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomNumber { get; set; }  // Oda numarası (örn: "401", "502")
    public int BedNumber { get; set; }
    public int? PatientId { get; set; }
    public string PatientName { get; set; } // Hastanın adı
    public string PatientSurname { get; set; } // Hastanın soyadı
    public string Status { get; set; }
    public DateTime? LastCleanedDate { get; set; }
    public DateTime CreatedAt { get; set; }
}