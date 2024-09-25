using AutoMapper;
using EFarma.Models;
using EFarma.Models.DTO;
using EFarma.Models.Resource;
using EFarma.Models.Views;

namespace EFarma.Config
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<PatientDTO, Patient>();
            CreateMap<RoleDTO, Role>();
            CreateMap<IEnumerable<Patient>, IEnumerable<PersonView>>();
            CreateMap<IEnumerable<Employee>, IEnumerable<PersonView>>();
        }
    }
}
