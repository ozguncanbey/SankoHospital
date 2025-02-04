using FluentNHibernate.Mapping;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class PatientMap : ClassMap<Patient>
{
    public PatientMap()
    {
        Table("Patients");
        LazyLoad();
        Id(x => x.Id).Column("id");
        Map(x => x.Name).Column("name");
        Map(x => x.Surname).Column("surname");
        Map(x => x.BloodType).Column("blood_type");
        Map(x => x.AdmissionDate).Column("admission_date");
        Map(x => x.CheckoutDate).Column("checkout_date");
    }
}