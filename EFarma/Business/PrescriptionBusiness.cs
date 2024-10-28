using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;

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

            var employee = await _repository.Employees.FirstOrDefault(m => m.Id == prescription.EmployeeId);
            if (employee == null)
            {
                return new ResultObject
                {
                    Message = $"Employee not found.",
                    StatusCode = 404,
                    Success = false
                };
            }
            else if (string.IsNullOrEmpty(employee.CRM))
            {
                return new ResultObject
                {
                    Message = $"Employee not allowed to prescript (CRM not registered).",
                    StatusCode = 403,
                    Success = false
                };
            }
            prescription.Employee = employee;

            var patient = await _repository.Patients.FirstOrDefault(m => m.CPF == prescription.CPF);
            if (patient == null)
            {
                return new ResultObject
                {
                    Message = $"Patient not found.",
                    StatusCode = 404,
                    Success = false
                };
            }
            prescription.Patient = patient;

            _repository.Prescriptions.Add(prescription);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Prescription created successfully." : "An error occurred while creating the prescription.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> DeletePrescription(int id)
        {
            var prescription = await _repository.Prescriptions.FirstOrDefault(e => e.Id == id);
            if (prescription == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Prescription not found.",
                    Success = false
                };
            }

            _repository.Prescriptions.Remove(prescription);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Prescription deleted successfully." : "An error occurred while deleting the prescription.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<PrescriptionItemView>>> GetPrescriptionItems(int prescriptionId)
        {
            var items = await _repository.PrescriptionItems.GetPrescriptionItems(prescriptionId);
            
            var prescriptionItemsView = items.Select(item => new PrescriptionItemView
            {
                Name = item.Medicament.Description,
                Dosage = $"{item.Medicament.Dosage} {item.Medicament.Measure}",
                Quantity = item.PrescribedQuantity
            }).ToList();

            bool has_items = prescriptionItemsView.Count != 0;
            return new()
            {
                Data = prescriptionItemsView,
                Message = has_items ? "Found prescription items" : "No items found for this prescription",
                StatusCode = has_items ? 200 : 404,
                Success = has_items
            };
        }

        public async Task<ResultDataObject<List<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date)
        {
            var detailedPrescriptions = await _repository.Prescriptions.GetDetailedPrescriptions(cpf, date);
            var prescriptions = _mapper.Map<List<PrescriptionView>>(detailedPrescriptions);

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
