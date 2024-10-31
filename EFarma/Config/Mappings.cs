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
                .ForMember(pa => pa.Role, 
                            opt => opt.MapFrom(src => "patient"));

            CreateMap<EmployeeDTO, Employee>();
            CreateMap<Employee, PersonView>()
                .ForMember(pa => pa.Role, 
                            opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<EntryLogDTO, AccessLog>();

            CreateMap<MedicamentDTO, Medicament>();

            CreateMap<PageDTO, Page>();

            CreateMap<PrescriptionDTO,  Prescription>();
            CreateMap<PrescriptionItemDTO,  PrescriptionItem>();
            CreateMap<Prescription, PrescriptionView>()
                .ForMember(pa => pa.PatientName, 
                            opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(pa => pa.DoctorName, 
                            opt => opt.MapFrom(src => src.Employee.Name));

            CreateMap<PermissionDTO, Permission>();
            CreateMap<Permission, PermissionView>();

            CreateMap<StockRoomDTO, StockRoom>();
            CreateMap<InStockItemDTO, InStockItem>();
            CreateMap<InStockItem, InStockItemView>()
                .ForMember(isi => isi.StockRoomName, 
                            opt => opt.MapFrom(src => src.StockRoom.Name))
                .ForMember(isi => isi.MedicamentDosage, 
                            opt => opt.MapFrom(src => $"{src.Medicament.Dosage}{src.Medicament.Measure}"))
                .ForMember(isi => isi.MedicamentName, 
                            opt => opt.MapFrom(src => src.Medicament.Description));
        }
    }
}
