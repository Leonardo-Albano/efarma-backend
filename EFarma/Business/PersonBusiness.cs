using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PersonBusiness : IPersonBusiness 
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public PersonBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultDataObject<IEnumerable<PersonView>>> GetPersonList(string? name, string? cpf)
        {
            var patients = await _repository.Patients
                .Find(p =>
               (!string.IsNullOrEmpty(name) && p.Name == name) ||
               (!string.IsNullOrEmpty(cpf) && p.CPF == cpf) ||
               (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cpf)));

            var employees = await _repository.Employees
                .Find(p =>
               (!string.IsNullOrEmpty(name) && p.Name == name) ||
               (!string.IsNullOrEmpty(cpf) && p.CPF == cpf) ||
               (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cpf)));


            var people = _mapper.Map<IEnumerable<PersonView>>(patients)
                            .Concat(_mapper.Map<IEnumerable<PersonView>>(employees));

            bool success = people.Any();

            return new()
            {
                Message = success ? "Found people." : "No people found.",
                Data = people,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }
    }
}
