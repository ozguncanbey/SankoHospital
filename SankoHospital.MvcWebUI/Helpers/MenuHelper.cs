namespace SankoHospital.MvcWebUI.Helpers;

public class MenuHelper
{
    public static string GetTurkishMenu(string menu)
    {
        return menu switch
        {
            "Dashboard" => "Panel",
            "Profile" => "Profil",
            "Settings" => "Ayarlar",
            _ => menu
        };
    }
}