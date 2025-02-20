using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.MvcWebUI.Models.CleanerModel;

namespace SankoHospital.MvcWebUI.Models.FilterModels;

public class BedListViewModel
{
    public List<BedViewModel> Beds { get; set; }

    // Filtreleme ile ilgili alanlar
    public int RoomNumber { get; set; } // Örneğin, oda numarası araması için
    public string SelectedStatus { get; set; }
    public string SearchTerm { get; set; }
    
    // Durum seçenekleri için liste (SelectListItem kullanabilirsiniz)
    public List<SelectListItem> StatusList { get; set; }
}