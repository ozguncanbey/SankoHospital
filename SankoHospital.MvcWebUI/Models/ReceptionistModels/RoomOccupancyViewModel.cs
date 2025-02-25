using SankoHospital.MvcWebUI.Models.CleanerModel;

namespace SankoHospital.MvcWebUI.Models.ReceptionistModel;

public class RoomOccupancyViewModel
{
    public RoomOccupancyViewModel()
    {
        Occupancy = new List<OccupancyViewModel>();
    }
    
    public RoomViewModel RoomInfo { get; set; } = new RoomViewModel();

    // List of daily records for the patient
    public List<OccupancyViewModel> Occupancy { get; set; }
}