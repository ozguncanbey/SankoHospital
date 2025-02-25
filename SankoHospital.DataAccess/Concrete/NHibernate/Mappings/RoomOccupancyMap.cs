using FluentNHibernate.Mapping;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class RoomOccupancyMap : ClassMap<RoomOccupancy>
{
    public RoomOccupancyMap()
    {
        Table("RoomOccupancy");

        Id(x => x.Id)
            .Column("id")
            .GeneratedBy.Identity();

        Map(x => x.RoomId)
            .Column("roomId")
            .Not.Nullable();

        Map(x => x.PatientId)
            .Column("patientId")
            .Not.Nullable();

        Map(x => x.AdmissionDate)
            .Column("admission_date")
            .Nullable();

        Map(x => x.CheckoutDate)
            .Column("checkout_date")
            .Nullable();

        Map(x => x.CreatedAt)
            .Column("created_at")
            .Not.Nullable();
    }
}