using EFarma.Business.Interfaces;
using EFarma.Models.Resource;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EFarma.Models.DTOs;
using EFarma.Models.Views;
using EFarma.Models.Response;
using MySqlX.XDevAPI.Common;

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
    }
}
