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
                .ForMember(pa => pa.Role, opt => opt.MapFrom("patient"));

            CreateMap<EmployeeDTO, Employee>();
            CreateMap<Employee, PersonView>()
                .ForMember(pa => pa.Role, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<EntryLogDTO, AccessLog>()
                .ForMember(el=>el.IsEntry, opt => opt.MapFrom(src => true));

            CreateMap<MedicamentDTO, Medicament>();

            CreateMap<PageDTO, Page>();

            CreateMap<PrescriptionDTO,  Prescription>();
            CreateMap<PrescriptionItemDTO,  PrescriptionItem>();
            CreateMap<Prescription, PrescriptionView>()
                .ForMember(pa => pa.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(pa => pa.DoctorName, opt => opt.MapFrom(src => src.Employee.Name));

            CreateMap<PermissionDTO, Permission>();
            CreateMap<Permission, PermissionView>();

            CreateMap<StockRoomDTO, StockRoom>();
            CreateMap<InStockItemDTO, InStockItem>();
        }
    }
}
