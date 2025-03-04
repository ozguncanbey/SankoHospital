using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class Bed : IEntity
{
    public virtual int Id { get; set; }
    public virtual int RoomId { get; set; }
    public virtual int BedNumber { get; set; }
    public virtual int? PatientId { get; set; } // Yatak boş olabilir, bu yüzden nullable
    public virtual string Status { get; set; }
    public virtual DateTime? LastCleanedDate { get; set; }
    public virtual DateTime CreatedAt { get; set; }
}