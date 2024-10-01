using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.Resource;
using EFarma.Models.Response;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

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
        public async Task<ActionResult<VoidResult>> CreatePatient([FromBody]PatientDTO patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);
            var result = await _business.CreatePatient(patient);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet("{cpf}")]
        public async Task<ActionResult<DataResult<Patient>>> GetPatient(string cpf)
        {
            var result = await _business.GetPatient(cpf);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet]
        public async Task<ActionResult<DataResult<IEnumerable<Patient>>>> GetAllPatients()
        {
            var result = await _business.GetAllPatients();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

    }
}
