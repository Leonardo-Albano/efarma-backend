using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Data;
using EFarma.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Business
{
    public class StatusCodeBusiness : IStatusCodeBusiness
    {
        private readonly ILogger<ControllerBase> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public StatusCodeBusiness(ILogger<ControllerBase> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
    }
}
