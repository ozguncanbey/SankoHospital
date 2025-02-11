using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class Patient : IEntity
{
    public virtual int Id { get; set; }              // Hasta ID (Primary Key)
    public virtual string Name { get; set; }        // Hasta Adı
    public virtual string Surname { get; set; }     // Hasta Soyadı
    public virtual string BloodType { get; set; }   // Kan Grubu (A+, B-, O+, vb.)
    public virtual DateTime AdmissionDate { get; set; } // Hastaneye giriş tarihi
    public virtual DateTime? CheckoutDate { get; set; } // Hastaneden çıkış tarihi (Nullable, çünkü bazı hastalar hala içeride olabilir)
    public virtual bool Checked { get; set; } // Hasta Kontrol Edildi mi?
}