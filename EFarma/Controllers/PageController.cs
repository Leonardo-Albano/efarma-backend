using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ILogger<PageController> _logger;
        private readonly IPageBusiness _business;
        private readonly IMapper _mapper;

        public PageController(ILogger<PageController> logger, IPageBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePage([FromBody] PageDTO pageDTO)
        {
            var result = await _business.CreatePage(pageDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<Page>>>> GetAllPages()
        {
            var result = await _business.GetAllPages();
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePage(int id)
        {
            var result = await _business.DeletePage(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
