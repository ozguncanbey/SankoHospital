namespace SankoHospital.MvcWebUI.Models.ReceptionistModel;

public class OccupancyViewModel
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; }
    public string PatientSurname { get; set; }
    public string BloodType { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? CheckoutDate { get; set; }
}