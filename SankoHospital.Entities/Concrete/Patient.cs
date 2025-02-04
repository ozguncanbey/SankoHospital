using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class Patient : IEntity
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Surname { get; set; }
    public virtual string BloodType { get; set; }
    public virtual DateTime AdmissionDate { get; set; }
    public virtual DateTime? CheckoutDate { get; set; }
}