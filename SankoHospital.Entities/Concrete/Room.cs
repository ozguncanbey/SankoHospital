using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class Room : IEntity
{
    public virtual int Id { get; set; }                     // Oda ID (Primary Key)
    public virtual int RoomNumber { get; set; }            // Oda Numarası (2 haneli)
    public virtual int Capacity { get; set; }              // Oda Kapasitesi (Kaç kişi alır?)
    public virtual int CurrentPatientCount { get; set; }   // Şu anda kaç hasta var?
    public virtual string Status { get; set; }            // Oda Durumu ('Cleaning', 'In Care', 'Cleaned', 'Waiting')
    public virtual DateTime? LastCleanedDate { get; set; } // Son temizlik tarihi
}