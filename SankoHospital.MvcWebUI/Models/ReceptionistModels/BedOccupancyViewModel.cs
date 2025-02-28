using SankoHospital.MvcWebUI.Models.CleanerModel;

namespace SankoHospital.MvcWebUI.Models.ReceptionistModel;

public class BedOccupancyViewModel
{
    public BedOccupancyViewModel()
    {
        Occupancy = new List<OccupancyViewModel>();
        Beds = new List<BedViewModel>();
    }

    public BedViewModel BedInfo { get; set; } = new BedViewModel();
    public List<OccupancyViewModel> Occupancy { get; set; }
    public List<BedViewModel> Beds { get; set; }
}