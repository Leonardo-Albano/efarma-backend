using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EFarma.Business
{
    public class PrescriptionBusiness : IPrescriptionBusiness
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public PrescriptionBusiness(ILogger<PrescriptionController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultObject> CreatePrescription(Prescription prescription)
        {
            prescription.Status = "Pendente";

            foreach (var item in prescription.Items)
            {
                var medicament = await _repository.Medicaments.FirstOrDefault(m => m.Id == item.MedicamentId);
                if (medicament == null)
                {
                    return new ResultObject
                    {
                        Message = $"Medicament with ID {item.MedicamentId} is not registered.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                item.Prescription = prescription;
                item.Medicament = medicament;
            }

            _repository.Prescriptions.Add(prescription);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Prescription created successfully." : "An error occurred while creating the prescription.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<IEnumerable<PrescriptionItemView>>> GetPrescriptionItems(int prescriptionId)
        {
            var items = await _repository.PrescriptionItems.GetPrescriptionItems(prescriptionId);
            
            var prescriptionItemsView = items.Select(item => new PrescriptionItemView
            {
                Name = item.Medicament.Description,
                Dosage = $"{item.Medicament.Dosage} {item.Medicament.Measure}",
                Quantity = item.PrescribedQuantity
            }).ToArray();

            bool has_items = prescriptionItemsView.Length != 0;
            return new()
            {
                Data = prescriptionItemsView,
                Message = has_items ? "Found prescription items" : "No items found for this prescription",
                StatusCode = has_items ? 200 : 404,
                Success = has_items
            };
        }

        public async Task<ResultDataObject<IEnumerable<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date)
        {
            var detailedPrescriptions = await _repository.Prescriptions.GetDetailedPrescriptions(cpf, date);
            var prescriptions = _mapper.Map<IEnumerable<PrescriptionView>>(detailedPrescriptions);

            bool success = prescriptions.Any();

            return new()
            {
                Message = success ? "Found prescriptions." : "No prescriptions found.",
                Data = prescriptions,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }
    }
}
