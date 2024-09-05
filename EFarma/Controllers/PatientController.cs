using EFarma.Models;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Patient>>> GetAllPatients()
        {
            List<Patient> patients = new()
            {
                new Patient
                {
                    CPF="123",
                    Name="Leo"
                }
            };

            return Ok(patients);
        }
    }
}
