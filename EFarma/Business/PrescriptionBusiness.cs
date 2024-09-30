using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
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

        public async Task<Prescription> CreatePrescription(Prescription prescription)
        {
            prescription.Status = "Pendente";
            foreach (var item in prescription.Items)
            {
                item.Prescription = prescription;
                item.Medicament = await _repository.Medicaments.FirstOrDefault(m=>m.Id == item.MedicamentId);
            }
            _repository.Prescriptions.Add(prescription);
            await _repository.SaveChangesAsync();
            return prescription;
        }

        public async Task<IEnumerable<PrescriptionView>> GetPrescriptions()
        {
            var prescriptions = await _repository.Prescriptions.GetDetailedPrescriptions();
            return _mapper.Map<IEnumerable<PrescriptionView>>(prescriptions);
        }

        public async Task<Patient?> GetPatient(string cpf)
            => await _repository.Patients.FirstOrDefault(p => p.CPF == cpf);

    }
}
