// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// roleMapping.js
const roleMap = {
    "Admin": "Yönetici",
    "User": "Kullanıcı",
    "Receptionist": "Resepsiyon",
    "Nurse": "Hemşire",
    "Cleaner": "Temizlik Görevlisi"
};

function getTurkishRole(role) {
    return roleMap[role] || role;
}
