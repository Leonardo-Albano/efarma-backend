using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Config;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EFarma.Business
{
    /// <summary>
    /// Classe responsável pelas operações relacionadas a funcionários, como criação, atualização e autenticação.
    /// </summary>
    public class EmployeeBusiness : PersonBusiness, IEmployeeBusiness
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="EmployeeBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="repository">Instância do repositório para manipulação de dados de funcionários.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        /// <param name="passwordHasher">Serviço de hashing de senhas.</param>
        public EmployeeBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository, IMapper mapper, IPasswordHasher passwordHasher, HttpClient httpClient)
            : base(logger, repository, mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Cria um novo funcionário, verificando se já existe um CPF ou código de tag duplicado.
        /// </summary>
        /// <param name="employee">Objeto do funcionário a ser criado.</param>
        /// <returns>Resultado da operação de criação com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> Create(Employee employee)
        {
            _logger.LogInformation("Iniciando criação de novo funcionário com CPF: {CPF} e código de tag: {TagCode}", employee.CPF, employee.TagCode);

            var existentEmployee = await _repository.Employees.Find(e => e.CPF == employee.CPF ||
                                     (!string.IsNullOrEmpty(employee.EmployeeId) && e.EmployeeId == employee.EmployeeId));
            if (existentEmployee.Count != 0)
            {
                _logger.LogWarning("CPF duplicado detectado para o CPF: {CPF}", employee.CPF);
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
                _logger.LogWarning("Código de tag duplicado detectado para o código: {TagCode}", employee.TagCode);
                return new ResultObject
                {
                    Message = "Este código de tag pertence a outro funcionário.",
                    StatusCode = 409,
                    Success = false
                };
            }

            if (!string.IsNullOrEmpty(employee.CRM) && await GetCrmData(employee.CRM) == null)
            {
                _logger.LogWarning("CRM inválido: {crm}", employee.CRM);
                return new ResultObject
                {
                    Message = "CRM inválido.",
                    StatusCode = 409,
                    Success = false
                };
            }

            employee.PasswordHash = _passwordHasher.Hash(employee.CPF.Replace(".", "").Replace("-", ""));
            _repository.Employees.Add(employee);

            bool success = await _repository.SaveChangesAsync() > 0;

            if (success)
            {
                _logger.LogInformation("Funcionário criado com sucesso com CPF: {CPF}", employee.CPF);
            }
            else
            {
                _logger.LogError("Erro ao salvar o novo funcionário com CPF: {CPF}", employee.CPF);
            }

            return new ResultObject
            {
                Message = success ? "Funcionário criado com sucesso." : "Ocorreu um erro ao criar o funcionário.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Obtém uma lista de todos os funcionários.
        /// </summary>
        /// <returns>Lista de funcionários com o status e mensagem apropriada.</returns>
        public async Task<ResultDataObject<List<Employee>>> GetAll()
        {
            _logger.LogInformation("Iniciando busca de todos os funcionários.");

            var employees = await _repository.Employees.GetAll();
            bool success = employees.Count != 0;

            if (success)
            {
                _logger.LogInformation("Funcionários encontrados: {EmployeeCount}", employees.Count);
            }
            else
            {
                _logger.LogWarning("Nenhum funcionário encontrado.");
            }

            return new()
            {
                Message = success ? "Funcionários encontrados." : "Nenhum funcionário encontrado.",
                Data = employees,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        /// <summary>
        /// Obtém um funcionário pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF do funcionário a ser localizado.</param>
        /// <returns>Objeto de resultado com o status e dados do funcionário, se encontrado.</returns>
        public async Task<ResultDataObject<Employee?>> GetByCPF(string cpf)
        {
            _logger.LogInformation("Iniciando busca do funcionário com CPF: {CPF}", cpf);

            var employee = await _repository.Employees.FirstOrDefault(e => e.CPF == cpf);
            bool success = employee != null;

            if (success)
            {
                _logger.LogInformation("Funcionário encontrado para o CPF: {CPF}", cpf);
            }
            else
            {
                _logger.LogWarning("Nenhum funcionário encontrado para o CPF: {CPF}", cpf);
            }

            return new()
            {
                Message = success ? "Funcionário encontrado." : "Nenhum funcionário encontrado.",
                Data = employee,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        /// <summary>
        /// Obtém um médico pelo número de CRM.
        /// </summary>
        /// <param name="crm">CRM do médico a ser localizado.</param>
        /// <returns>Objeto de resultado com o status e dados do médico, se encontrado.</returns>
        public async Task<ResultDataObject<Employee?>> GetDoctorByCrm(string crm)
        {
            _logger.LogInformation("Iniciando busca do médico com CRM: {CRM}", crm);

            var doctor = await _repository.Employees.FirstOrDefault(e => e.CRM == crm);
            bool success = doctor != null;

            if (success)
            {
                _logger.LogInformation("Médico encontrado para o CRM: {CRM}", crm);
            }
            else
            {
                _logger.LogWarning("Nenhum médico encontrado para o CRM: {CRM}", crm);
            }

            return new()
            {
                Message = success ? "Médico encontrado." : "Nenhum médico encontrado.",
                Data = doctor,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        /// <summary>
        /// Valida o CRM de um médico e obtém suas informações, se encontrado.
        /// </summary>
        /// <param name="crm">Número do CRM do médico a ser validado.</param>
        /// <returns>Objeto de resultado contendo as informações do médico, status da operação e mensagem apropriada.</returns>
        public async Task<ResultDataObject<DoctorInfoView?>> ValidateCrm(string crm)
        {
            var doctorInfoView = await GetCrmData(crm);
            var result = doctorInfoView != null;

            return new()
            {
                Data = result ? doctorInfoView : null,
                Message = result ? "Doutor(a) encontrado." : "Doutor(a) não foi encontrado.",
                StatusCode = result ? 200 : 404,
                Success = result
            };
        }

        /// <summary>
        /// Atualiza os dados de um funcionário pelo ID.
        /// </summary>
        /// <param name="id">ID do funcionário a ser atualizado.</param>
        /// <param name="employeeDto">Dados atualizados do funcionário.</param>
        /// <returns>Resultado da operação de atualização com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> Update(int id, EmployeeDTO employeeDto)
        {
            var existingEmployeeResult = await _repository.Employees.FirstOrDefault(e => e.Id == id);

            if (existingEmployeeResult == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Funcionário não foi encontrado."
                };
            }

            _mapper.Map(employeeDto, existingEmployeeResult);

            _repository.Employees.Update(existingEmployeeResult);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Funcionário atualizado com sucesso." : "Ocorreu um erro ao atualizar o funcionário.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Exclui um funcionário pelo ID.
        /// </summary>
        /// <param name="id">ID do funcionário a ser excluído.</param>
        /// <returns>Resultado da operação de exclusão com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> Delete(int id)
        {
            _logger.LogInformation("Iniciando exclusão do funcionário com ID: {EmployeeId}", id);

            var employee = await _repository.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                _logger.LogWarning("Funcionário não encontrado para o ID: {EmployeeId}", id);
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Funcionário não encontrado.",
                    Success = false
                };
            }

            _repository.Employees.Remove(employee);
            bool success = await _repository.SaveChangesAsync() > 0;

            if (success)
            {
                _logger.LogInformation("Funcionário excluído com sucesso para o ID: {EmployeeId}", id);
            }
            else
            {
                _logger.LogError("Erro ao excluir o funcionário para o ID: {EmployeeId}", id);
            }

            return new ResultObject
            {
                Message = success ? "Funcionário excluído com sucesso." : "Ocorreu um erro ao excluir o funcionário.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Exporta uma lista de todos os funcionários cadastrados no sistema em formato CSV.
        /// </summary>
        /// <returns>Resultado da operação de exportação contendo o arquivo CSV, o status e a mensagem apropriada.</returns>
        public async Task<ResultDataObject<byte[]?>> Export()
        {
            _logger.LogInformation("Iniciando exportação de funcionários em formato CSV.");

            try
            {
                var employees = await _repository.Employees.GetAllDetailed(); ;

                if (employees.Count == 0)
                {
                    _logger.LogWarning("Nenhum funcionário encontrado para exportação.");
                    return new ResultDataObject<byte[]?>
                    {
                        Data = null,
                        Success = false,
                        Message = "Nenhum funcionário encontrado para exportação.",
                        StatusCode = 404
                    };
                }

                var csvContent = GenerateCsvContent(employees);
                _logger.LogInformation("Exportação de funcionários concluída com sucesso.");

                return new ResultDataObject<byte[]?>
                {
                    Success = true,
                    Data = csvContent,
                    Message = "Arquivo CSV exportado com sucesso.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao exportar funcionários: {ExceptionMessage}", ex.Message);
                return new ResultDataObject<byte[]?>
                {
                    Data = null,
                    Success = false,
                    Message = "Erro ao exportar o arquivo CSV.",
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// Importa uma lista de funcionários a partir de um arquivo CSV. O arquivo deve conter os campos na mesma estrutura do CSV exportado.
        /// Observação: A função deve ser criadas antes de importar os funcionários, pois são referenciadas pelo nome passado no arquivo.
        /// </summary>
        /// <param name="csvData">Array de bytes representando o conteúdo do arquivo CSV.</param>
        /// <returns>Resultado da operação de importação, incluindo os funcionários criados e uma lista de entradas incorretas.</returns>
        public async Task<ResultDataObject<List<ImportError>>> ImportEmployees(byte[] csvData)
        {
            _logger.LogInformation("Iniciando importação de funcionários a partir de CSV.");

            var createdEmployees = new List<Employee>();
            var invalidEntries = new List<ImportError>();

            try
            {
                using var reader = new StreamReader(new MemoryStream(csvData));
                var header = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) 
                        continue;

                    var importErrorBase = new ImportError() { Line = line };

                    var values = line.Split(',');

                    if (values.Length < 9)
                    {
                        _logger.LogWarning("Linha com dados insuficientes: {Line}", line);
                        importErrorBase.Error = "Linha com colunas insuficientes";
                        invalidEntries.Add(importErrorBase);
                        continue;
                    }

                    try
                    {
                        var roleName = values[8];
                        var role = await _repository.Roles.GetRoleByName(roleName);

                        if (role == null)
                        {
                            string message = $"Função não encontrada para o nome: {roleName}";
                            _logger.LogWarning(message);
                            importErrorBase.Error = message;

                            invalidEntries.Add(importErrorBase);
                            continue;
                        }

                        var existentEmployee = await _repository.Employees.GetFiltered(values[2], null);

                        if(existentEmployee != null)
                        {
                            var message = $"Funcionário com esse CPF já foi cadastrado: {values[2]}";
                            _logger.LogWarning(message);
                            importErrorBase.Error = message;
                            invalidEntries.Add(importErrorBase);
                            continue;
                        }

                        var employee = new Employee
                        {
                            EmployeeId = values[0],
                            Name = values[1],
                            CPF = values[2],
                            PasswordHash = _passwordHasher.Hash(values[2].Replace(".", "").Replace("-", "")),
                            BirthDate = DateTime.Parse(values[3]),
                            Mail = values[4],
                            ResponsibleMail = values[5],
                            CRM = values[6],
                            TagCode = values[7],
                            Role = role
                        };

                        _repository.Employees.Add(employee);
                        createdEmployees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Erro ao processar a linha: {Line}. Erro: {ExceptionMessage}", line, ex.Message);
                        importErrorBase.Error = $"Exceção ao processar a linha: {ex.Message}";
                        invalidEntries.Add(importErrorBase);
                    }
                }

                bool success = await _repository.SaveChangesAsync() > 0;
                _logger.LogInformation("Importação de funcionários concluída com sucesso. Funcionários criados: {Count}", createdEmployees.Count);

                return new ResultDataObject<List<ImportError>>
                {
                    Success = success,
                    Message = success ? "Funcionários importados com sucesso." : "Ocorreu um erro ao salvar os funcionários.",
                    StatusCode = success ? 200 : 500,
                    Data = invalidEntries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao importar funcionários: {ExceptionMessage}", ex.Message);
                return new ResultDataObject<List<ImportError>>
                {
                    Success = false,
                    Data = invalidEntries,
                    Message = "Erro ao importar o arquivo CSV.",
                    StatusCode = 500,
                };
            }
        }

        // AUTH

        /// <summary>
        /// Realiza o login de um funcionário, verificando as credenciais fornecidas.
        /// </summary>
        /// <param name="loginDTO">Dados do login, incluindo email e senha.</param>
        /// <returns>Resultado da operação de login com status e mensagem apropriada.</returns>
        public async Task<ResultDataObject<Employee?>> Login(LoginDTO loginDTO)
        {
            var employee = await _repository.Employees.GetEmployeeDetailedByMail(loginDTO.Mail);
            if (employee == null)
            {
                return new()
                {
                    Data = null,
                    Message = "Funcionário não encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var result = _passwordHasher.Verify(employee.PasswordHash, loginDTO.Password);

            return new()
            {
                Data = result ? employee : null,
                Message = result ? "Acesso permitido." : "Acesso negado.",
                StatusCode = result ? 200 : 403,
                Success = result
            };
        }

        /// <summary>
        /// Redefine a senha de um funcionário para o hash do seu CPF.
        /// </summary>
        /// <param name="employeeId">ID do funcionário cuja senha será redefinida.</param>
        /// <returns>Resultado da operação de redefinição de senha com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> ResetPassword(int employeeId)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                return new()
                {
                    Message = "Funcionário não encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            employee.PasswordHash = _passwordHasher.Hash(employee.CPF.Replace(".", "").Replace("-", ""));

            _repository.Employees.Update(employee);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Senha redefinida com sucesso." : "Ocorreu um erro ao redefinir a senha.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }


        /// <summary>
        /// Atualiza a senha de um funcionário, validando a senha antiga e a nova.
        /// </summary>
        /// <param name="updatePasswordDTO">Dados para atualização da senha: email, senha antiga e nova senha.</param>
        /// <returns>Resultado da operação de atualização de senha com o status e mensagem apropriada.</returns>
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

        /// <summary>
        /// Gera o conteúdo CSV para exportação de uma lista de funcionários.
        /// </summary>
        /// <param name="employees">Lista de funcionários a serem exportados.</param>
        /// <returns>Arquivo CSV em formato de array de bytes.</returns>
        private static byte[] GenerateCsvContent(IEnumerable<Employee> employees)
        {
            var csv = new StringBuilder();
            csv.AppendLine("IdFuncionario,Nome,CPF,DataDeNascimento,Email,EmailDoResponsavel,CRM,CodigoCracha,Cargo");

            foreach (var employee in employees)
            {
                csv.AppendLine($"{employee.EmployeeId},{employee.Name},{employee.CPF},{employee.BirthDate},{employee.Mail},{employee.ResponsibleMail},{employee.CRM},{employee.TagCode},{employee.Role.Name}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        /// <summary>
        /// Recupera os dados do CRM do médico a partir de uma API externa do Conselho Federal de Medicina.
        /// </summary>
        /// <param name="crm">Número do CRM do médico.</param>
        /// <returns>Objeto <see cref="DoctorInfoView"/> com as informações do médico, se encontrado; caso contrário, <c>null</c>.</returns>
        private async Task<DoctorInfoView> GetCrmData(string crm)
        {
            string state = Regex.Replace(crm, "[^a-zA-Z]", "");
            string registry = Regex.Replace(crm, "[^0-9]", "");

            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://portal.cfm.org.br/api_rest_php/api/v1/medicos/buscar_medicos");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            var jsonPayload = $"[{{\"medico\":{{\"nome\":\"\",\"ufMedico\":\"{state}\",\"crmMedico\":\"{registry}\",\"municipioMedico\":\"\",\"tipoInscricaoMedico\":\"\",\"situacaoMedico\":\"\",\"detalheSituacaoMedico\":\"\",\"especialidadeMedico\":\"\",\"areaAtuacaoMedico\":\"\"}},\"page\":1,\"pageNumber\":1,\"pageSize\":10}}]";
            var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var unescapedResponse = JsonDocument.Parse(responseBody).RootElement.ToString();

            var doctorInfoDto = JsonSerializer.Deserialize<DoctorInfoDto>(unescapedResponse);
            var success = doctorInfoDto?.Status == "sucesso" && doctorInfoDto?.Dados.Count > 0;
            return success ? _mapper.Map<DoctorInfoView>(doctorInfoDto.Dados[0]) : null;
        }
    }
}