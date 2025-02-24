using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class PatientDailyRecord : IEntity
{
    public virtual int Id { get; set; }    
    public virtual int PatientId { get; set; }  
    public virtual DateTime RecordDate { get; set; }
    public virtual string? BloodPressure { get; set; }  // Tansiyon, örn. "120/80"
    public virtual int? Pulse { get; set; }             // Nabız sayısı
    public virtual string? BloodSugar { get; set; }       // Kan şekeri değeri (format tercihinize göre)
    public virtual DateTime CreatedAt { get; set; }
}