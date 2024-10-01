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
            CreateMap<RoleDTO, Role>();

            CreateMap<PatientDTO, Patient>();
            CreateMap<Patient, PersonView>()
                .ForMember(pa => pa.Role, opt => opt.MapFrom(src => "patient"));

            CreateMap<Employee, PersonView>();
            CreateMap<EmployeeDTO, Employee>();


            CreateMap<PrescriptionDTO,  Prescription>();
            CreateMap<PrescriptionItemDTO,  PrescriptionItem>();

        }
    }
}
