using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class BedOccupancy : IEntity
{
    public virtual int Id { get; set; }
    public virtual int RoomId { get; set; }
    public virtual int PatientId { get; set; }
    public virtual int BedId { get; set; }
    public virtual DateTime AdmissionDate { get; set; }
    public virtual DateTime? CheckoutDate { get; set; }
    public virtual DateTime CreatedAt { get; set; }
}