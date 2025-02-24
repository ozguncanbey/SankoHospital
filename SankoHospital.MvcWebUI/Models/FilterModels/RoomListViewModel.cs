using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.MvcWebUI.Models.CleanerModel;

namespace SankoHospital.MvcWebUI.Models.FilterModels;

public class RoomListViewModel
{
    // Listelenen odalar
    public List<RoomViewModel> Rooms { get; set; }
        
    // Filtreleme alanları:
    public int? Id { get; set; }
    public int? RoomNumber { get; set; }        // Oda numarası
    public int? Capacity { get; set; }            // Kapasite
    public int? CurrentPatientCount { get; set; }
    public string Status { get; set; } // Mevcut hasta sayısı
    public string SelectedStatus { get; set; }    // Durum (Cleaned, Cleaning, In Care, Waiting)
    public DateTime? LastCleanedDate { get; set; }  // Son temizlenme tarihi
    //public string SelectedOccupancy { get; set; }   // Doluluk durumu (Full, Empty)
        
    // Dropdown seçenekleri:
    public List<SelectListItem> StatusList { get; set; }
}