namespace SankoHospital.MvcWebUI.Helpers;

public class CleanerHelper
{
    public static string GetTurkishStatus(string status)
    {
        return status switch
        {
            "Cleaned" => "Temizlendi",
            "Cleaning" => "Temizleniyor",
            "In Care" => "Bakımda",
            "Waiting" => "Bekliyor",
            _ => status
        };
    }
}