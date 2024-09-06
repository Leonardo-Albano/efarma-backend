using EFarma.Business.Interfaces;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientBusiness _business;

        public PatientController(ILogger<PatientController> logger, IPatientBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            IEnumerable<Patient> patients = _business.GetAllPatients();

            if (!patients.Any())
            {
                return NotFound();
            }

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            var patient = new Patient
            {
                CPF = "123",
                Name = "Leo"
            };

            return Ok(patient);
        }
    }
}
