using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Config;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class EmployeeBusiness : PersonBusiness, IEmployeeBusiness
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public EmployeeBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository, IMapper mapper, IPasswordHasher passwordHasher) : base(logger, repository, mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResultObject> CreateEmployee(Employee employee)
        {
            var existentEmployee = await _repository.Employees.Find(e => e.CPF == employee.CPF ||
                                     (!string.IsNullOrEmpty(employee.EmployeeId) && e.EmployeeId == employee.EmployeeId));
            if (existentEmployee.Any())
            {
                return new ResultObject
                {
                    Message = "CPF já existe no sistema.",
                    StatusCode = 409,
                    Success = false
                };
            }

            var existentTagCode = await _repository.Employees.GetEmployeeByTagCode(employee.TagCode);
            if (existentTagCode != null)
            {
                return new ResultObject
                {
                    Message = "Este código de tag pertence a outro funcionário.",
                    StatusCode = 409,
                    Success = false
                };
            }

            employee.PasswordHash = _passwordHasher.Hash(employee.CPF.Replace(".", "").Replace("-", ""));
            _repository.Employees.Add(employee);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Funcionário criado com sucesso." : "Ocorreu um erro ao criar o funcionário.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> DeleteEmployee(int id)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Funcionário não encontrado.",
                    Success = false
                };
            }

            _repository.Employees.Remove(employee);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Funcionário excluído com sucesso." : "Ocorreu um erro ao excluir o funcionário.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<Employee?>> GetDoctorByCrm(string crm)
        {
            var doctor = await _repository.Employees.FirstOrDefault(e => e.CRM == crm);
            bool success = doctor != null;

            return new()
            {
                Message = success ? "Médico encontrado." : "Nenhum médico encontrado.",
                Data = doctor,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultDataObject<Employee>> GetEmployee(string cpf)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.CPF == cpf);
            bool success = employee != null;

            return new()
            {
                Message = success ? "Funcionário encontrado." : "Nenhum funcionário encontrado.",
                Data = employee,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultDataObject<List<Employee>>> GetEmployees()
        {
            var employees = await _repository.Employees.GetAll();

            bool success = employees.Any();

            return new()
            {
                Message = success ? "Funcionários encontrados." : "Nenhum funcionário encontrado.",
                Data = employees,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        public async Task<ResultObject> Login(LoginDTO loginDTO)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.Mail == loginDTO.Mail);
            if (employee == null)
            {
                return new()
                {
                    Message = "Funcionário não encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var result = _passwordHasher.Verify(employee.PasswordHash, loginDTO.Password);

            return new()
            {
                Message = result ? "Acesso permitido." : "Acesso negado.",
                StatusCode = result ? 200 : 403,
                Success = result
            };
        }

        public async Task<ResultDataObject<Employee?>> UpdateEmployee(Employee employee)
        {
            _repository.Employees.Update(employee);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultDataObject<Employee?>
            {
                Message = success ? "Funcionário atualizado com sucesso." : "Ocorreu um erro ao atualizar o funcionário.",
                Data = success ? employee : null,
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> UpdatePassword(EmployeeUpdatePasswordDTO updatePasswordDTO)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.Mail == updatePasswordDTO.Mail);
            if (employee == null)
            {
                return new()
                {
                    Message = "Funcionário não encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var tryLogin = await Login(new()
            {
                Mail = updatePasswordDTO.Mail,
                Password = updatePasswordDTO.OldPassword
            });

            if (!tryLogin.Success)
            {
                return new()
                {
                    Message = "Senha incorreta.",
                    StatusCode = 403,
                    Success = false
                };
            }

            if (string.IsNullOrEmpty(updatePasswordDTO.NewPassword))
            {
                return new()
                {
                    Message = "Senha inválida.",
                    StatusCode = 403,
                    Success = false
                };
            }

            employee.PasswordHash = _passwordHasher.Hash(updatePasswordDTO.NewPassword);
            _repository.Employees.Update(employee);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Senha atualizada com sucesso." : "Ocorreu um erro ao atualizar a senha.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}
