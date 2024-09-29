using AutoMapper;
using EFarma.Models;
using EFarma.Models.DTOs;
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
            CreateMap<Patient, PersonView>()
                .ForMember(pa => pa.Role, opt => opt.MapFrom(src => "patient"));
            CreateMap<Employee, PersonView>();

            CreateMap<PrescriptionDTO,  Prescription>();
            CreateMap<PrescriptionItemDTO,  PrescriptionItem>();

        }
    }
}
