using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.Resource;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientBusiness _business;
        private readonly IMapper _mapper;

        public PatientController(ILogger<PatientController> logger, IPatientBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreatePatient([FromBody]PatientDTO patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);
            var result = await _business.CreatePatient(patient);
            return StatusCode(result);
        }

        [HttpGet("{cpf}")]
        public async Task<ActionResult<Patient>> GetPatient(string cpf)
        {
            var patient = await _business.GetPatient(cpf);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            IEnumerable<Patient> patients = await _business.GetAllPatients();

            if (!patients.Any())
            {
                return NotFound();
            }

            return Ok(patients);
        }

    }
}
