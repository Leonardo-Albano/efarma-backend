using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IPrescriptionBusiness _business;
        private readonly IMapper _mapper;

        public PrescriptionController(ILogger<PrescriptionController> logger, IPrescriptionBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePrescription([FromBody] PrescriptionDTO prescriptionDTO)
        {
            var prescription = _mapper.Map<Prescription>(prescriptionDTO);
            var result = await _business.CreatePrescription(prescription);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<PrescriptionView>>>> GetPrescriptions(string? cpf, DateTime? date)
        {
            var result = await _business.GetPrescriptions(cpf, date);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet("{prescriptionId}")]
        public async Task<ActionResult<ResultDataObject<IEnumerable<PrescriptionItemView>>>> GetPrescriptionItems(int prescriptionId)
        {
            var result = await _business.GetPrescriptionItems(prescriptionId);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePrescription(int id)
        {
            var result = await _business.DeletePrescription(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

    }
}
