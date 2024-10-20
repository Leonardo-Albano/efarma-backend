using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
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
        public async Task<ActionResult<ResultObject>> CreatePatient([FromBody]PatientDTO patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);
            var result = await _business.CreatePatient(patient);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet("{cpf}")]
        public async Task<ActionResult<ResultDataObject<Patient>>> GetPatient(string cpf)
        {
            var result = await _business.GetPatient(cpf);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<Patient>>>> GetAllPatients()
        {
            var result = await _business.GetAllPatients();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpPut]
        public async Task<ActionResult<ResultDataObject<Patient?>>> UpdatePatient([FromBody] PatientDTO patientDto)
        {
            var existingPatientResult = await _business.GetPatient(patientDto.CPF);

            if (existingPatientResult == null || !existingPatientResult.Success && existingPatientResult.Data == null)
            {
                return NotFound(new ResultDataObject<Patient?>
                {
                    StatusCode = 404,
                    Message = "Patient not found.",
                    Data = null
                });
            }

            var existingPatient = existingPatientResult.Data;

            _mapper.Map(patientDto, existingPatient);
            var result = await _business.UpdatePatient(existingPatient);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePatient(int id)
        {
            var result = await _business.DeletePatient(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}
