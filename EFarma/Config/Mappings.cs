using AutoMapper;
using EFarma.Models;
using EFarma.Models.DTO;
using EFarma.Models.Resource;

namespace EFarma.Config
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<PatientDTO, Patient>();
            CreateMap<RoleDTO, Role>();
        }
    }
}
