namespace SankoHospital.MvcWebUI.Helpers;

public static class RoleHelper
{
    public static string GetTurkishRole(string role)
    {
        return role switch
        {
            "Admin" => "Yönetici",
            "User" => "Kullanıcı",
            "Receptionist" => "Resepsiyonist",
            "Nurse" => "Hemşire",
            "Cleaner" => "Temizlik Görevlisi",
            _ => role
        };
    }
}