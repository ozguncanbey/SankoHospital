namespace SankoHospital.MvcWebUI.Models;

public class PatientViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string BloodType { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? CheckoutDate { get; set; }
    
    public bool Checked { get; set; }
}