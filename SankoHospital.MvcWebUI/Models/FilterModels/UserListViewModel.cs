using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Models.FilterModels;

public class UserListViewModel
{
    // Listelenen kullanıcılar
    public List<UserViewModel> Users { get; set; } = new();

    // Filtreleme ile ilgili alanlar
    public int? Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string SelectedRole { get; set; } = string.Empty;

    // Rol seçenekleri (dropdown için)
    public List<SelectListItem> RoleList { get; set; } = new();
}