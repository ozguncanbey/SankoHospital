using FluentNHibernate.Mapping;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class BedOccupancyMap : ClassMap<BedOccupancy>
{
    public BedOccupancyMap()
    {
        Table("BedOccupancy");

        Id(x => x.Id)
            .Column("id")
            .GeneratedBy.Identity();

        Map(x => x.RoomId)
            .Column("roomId")
            .Not.Nullable();

        Map(x => x.PatientId)
            .Column("patientId")
            .Not.Nullable();

        Map(x => x.BedId)
            .Column("bedId")
            .Not.Nullable();

        Map(x => x.AdmissionDate)
            .Column("admission_date")
            .Not.Nullable();

        Map(x => x.CheckoutDate)
            .Column("checkout_date")
            .Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();
    }
}