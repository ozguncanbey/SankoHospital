using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.MvcWebUI.Models.NurseModel;

namespace SankoHospital.MvcWebUI.Models.FilterModels;

public class PatientListViewModel
{
    // Liste halinde hastalar
    public List<PatientViewModel> Patients { get; set; }

    // Filtreleme ile ilgili alanlar
    public int? Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string BloodType { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public int? RoomId { get; set; }
    public int? RoomNumber { get; set; }

    // Dropdown seçenekleri (opsiyonel)
    public List<SelectListItem> BloodTypeList { get; set; }
}