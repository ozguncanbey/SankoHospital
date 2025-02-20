using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.MvcWebUI.Models.CleanerModel;

namespace SankoHospital.MvcWebUI.Models.FilterModels;

public class BedListViewModel
{
    public List<BedViewModel> Beds { get; set; }

    // Filtreleme ile ilgili alanlar
    public int? Id { get; set; }              // Yatak ID'si filtrelemesi için
    public int? RoomNumber { get; set; }        // Oda numarası (RoomNumber) filtrelemesi için
    public int? BedNumber { get; set; }         // Yatak numarası filtrelemesi için
    public int? PatientId { get; set; }         // Hasta ID filtrelemesi için
    public string SelectedStatus { get; set; }  // Durum filtrelemesi için
    public DateTime? LastCleanedDate { get; set; } // Son temizlik tarihi filtrelemesi için
    
    // Durum seçenekleri için liste
    public List<SelectListItem> StatusList { get; set; }
}