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

        public async Task<VoidResult> CreatePrescription(Prescription prescription)
        {
            prescription.Status = "Pendente";

            foreach (var item in prescription.Items)
            {
                var medicament = await _repository.Medicaments.FirstOrDefault(m => m.Id == item.MedicamentId);
                if (medicament == null)
                {
                    return new VoidResult
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
            return new VoidResult
            {
                Message = success ? "Prescription created successfully." : "An error occurred while creating the prescription.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<DataResult<IEnumerable<PrescriptionView>>> GetPrescriptions()
        {
            var detailedPrescriptions = await _repository.Prescriptions.GetDetailedPrescriptions();
            var prescriptions = _mapper.Map<IEnumerable<PrescriptionView>>(detailedPrescriptions);

            bool success = prescriptions.Any();

            return new()
            {
                Message = success ? "Found prescriptions." : "No prescriptions found.",
                Result = prescriptions,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }
    }
}
