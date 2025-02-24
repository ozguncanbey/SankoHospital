using FluentNHibernate.Mapping;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class PatientDailyRecordMap : ClassMap<PatientDailyRecord>
{
    public PatientDailyRecordMap()
    {
        Table("PatientDailyRecord");
        LazyLoad();
        Id(x => x.Id).Column("id");
        Map(x => x.PatientId).Column("patient_id");
        Map(x => x.RecordDate).Column("record_date");
        Map(x => x.BloodPressure).Column("blood_pressure").Nullable();
        Map(x => x.Pulse).Column("pulse").Nullable();
        Map(x => x.BloodSugar).Column("blood_sugar").Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
    }
}