using EFarma.Business.Interfaces;
using EFarma.Models.Resource;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EFarma.Models.DTOs;
using EFarma.Models.Views;

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
        public async Task<ActionResult> CreatePrescription([FromBody] PrescriptionDTO prescriptionDTO)
        {
            var prescription = _mapper.Map<Prescription>(prescriptionDTO);
            var result = await _business.CreatePrescription(prescription);
            if(result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionView>>> GetPrescriptions()
        {
            IEnumerable<PrescriptionView> prescriptions = await _business.GetPrescriptions();

            if (!prescriptions.Any())
            {
                return NotFound();
            }

            return Ok(prescriptions);
        }
    }
}
