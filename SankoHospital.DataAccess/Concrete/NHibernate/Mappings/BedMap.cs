using FluentNHibernate.Mapping;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class BedMap : ClassMap<Bed>
{
    public BedMap()
    {
        Table("Beds");
        LazyLoad();
        Id(x => x.Id).Column("id");
        Map(x => x.RoomId).Column("roomId");
        Map(x => x.BedNumber).Column("bed_number");
        Map(x => x.PatientId).Column("patientId").Nullable();
        Map(x => x.Status).Column("status");
        Map(x => x.LastCleanedDate).Column("last_cleaned_date").Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
    }
}