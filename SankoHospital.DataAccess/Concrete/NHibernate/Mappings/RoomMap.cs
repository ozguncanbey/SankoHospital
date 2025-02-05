using FluentNHibernate.Mapping;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class RoomMap : ClassMap<Room>
{
    public RoomMap()
    {
        Table("Rooms");
        LazyLoad();
        Id(x => x.Id).Column("id");
        Map(x => x.RoomNumber).Column("room_number");
        Map(x => x.Capacity).Column("capacity");
        Map(x => x.CurrentPatientCount).Column("current_patient_count");
        Map(x => x.Status).Column("status");
        Map(x => x.LastCleanedDate).Column("last_cleaned_date");
    }
}