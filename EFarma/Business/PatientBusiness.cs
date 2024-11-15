using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;
using System.Text;

namespace EFarma.Business
{
    public class PatientBusiness : IPatientBusiness
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public PatientBusiness(ILogger<PatientController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultObject> CreatePatient(Patient patient)
        {
            var existent_patients = await _repository.Patients.FirstOrDefault(p => p.CPF == patient.CPF);
            if (existent_patients != null)
            {
                return new ResultObject
                {
                    Message = "CPF já existe no sistema.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Patients.Add(patient);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Paciente criado com sucesso." : "Ocorreu um erro ao criar o paciente.",
                Success = success,
                StatusCode = success ? 200 : 500
            };
        }

        public async Task<ResultObject> DeletePatient(int id)
        {
            var patient = await _repository.Patients.FirstOrDefault(e => e.Id == id);
            if (patient == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Paciente não encontrado.",
                    Success = false
                };
            }

            _repository.Patients.Remove(patient);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Paciente excluído com sucesso." : "Ocorreu um erro ao excluir o paciente.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<Patient>>> GetAllPatients()
        {
            var patients = await _repository.Patients.GetAll();

            bool success = patients.Any();

            return new()
            {
                Message = success ? "Pacientes encontrados." : "Nenhum paciente encontrado.",
                Data = patients,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultDataObject<Patient>> GetPatient(string cpf)
        {
            var patient = await _repository.Patients.FirstOrDefault(p => p.CPF == cpf);
            bool success = patient != null;

            return new()
            {
                Message = success ? "Paciente encontrado." : "Nenhum paciente encontrado.",
                Data = patient,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultDataObject<Patient?>> UpdatePatient(Patient patient)
        {
            _repository.Patients.Update(patient);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultDataObject<Patient?>
            {
                Message = success ? "Paciente atualizado com sucesso." : "Ocorreu um erro ao atualizar o paciente.",
                Data = success ? patient : null,
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Exporta uma lista de todos os pacientes cadastrados no sistema em formato CSV.
        /// </summary>
        /// <returns>Resultado da operação de exportação contendo o arquivo CSV, o status e a mensagem apropriada.</returns>
        public async Task<ResultDataObject<byte[]?>> ExportPatients()
        {
            _logger.LogInformation("Iniciando exportação de pacientes em formato CSV.");

            try
            {
                var patients = await _repository.Patients.GetAll(); ;

                if (patients.Count == 0)
                {
                    _logger.LogWarning("Nenhum paciente encontrado para exportação.");
                    return new ResultDataObject<byte[]?>
                    {
                        Data = null,
                        Success = false,
                        Message = "Nenhum paciente encontrado para exportação.",
                        StatusCode = 404
                    };
                }

                var csvContent = GenerateCsvContent(patients);
                _logger.LogInformation("Exportação de pacientes concluída com sucesso.");

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
                _logger.LogError("Erro ao exportar pacientes: {ExceptionMessage}", ex.Message);
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
        /// Importa uma lista de pacientes a partir de um arquivo CSV. O arquivo deve conter os campos na mesma estrutura do CSV exportado.
        /// Observação: As funções devem ser criadas antes de importar pacientes, pois são referenciadas pelo nome no arquivo.
        /// </summary>
        /// <param name="csvData">Array de bytes representando o conteúdo do arquivo CSV.</param>
        /// <returns>Resultado da operação de importação, incluindo os pacientes criados e uma lista de entradas incorretas.</returns>
        public async Task<ResultDataObject<List<string>>> ImportPatients(byte[] csvData)
        {
            _logger.LogInformation("Iniciando importação de pacientes a partir de CSV.");

            var createdPatients = new List<Patient>();
            var invalidEntries = new List<string>();

            try
            {
                using var reader = new StreamReader(new MemoryStream(csvData));
                var header = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    var values = line.Split(',');

                    if (values.Length < 6)
                    {
                        _logger.LogWarning("Linha com dados insuficientes: {Line}", line);
                        invalidEntries.Add(line);
                        continue;
                    }

                    try
                    {
                        var existentPatients = await _repository.Patients.GetByCPF(values[1]);

                        if (existentPatients == null)
                        {
                            _logger.LogWarning("Funcionário com esse CPF já foi cadastrado: {Cpf}", values[2]);
                            invalidEntries.Add(line);
                            continue;
                        }

                        var patient = new Patient
                        {
                            Name = values[0],
                            CPF = values[1],
                            BirthDay = DateTime.Parse(values[3]),
                            Mail = values[4],
                            PhoneNumber = values[5],
                            Observations = values[6]
                        };

                        _repository.Patients.Add(patient);
                        createdPatients.Add(patient);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Erro ao processar a linha: {Line}. Erro: {ExceptionMessage}", line, ex.Message);
                        invalidEntries.Add(line);
                    }
                }

                bool success = await _repository.SaveChangesAsync() > 0;
                _logger.LogInformation("Importação de pacientes concluída com sucesso. Pacientes criados: {Count}", createdPatients.Count);

                return new ResultDataObject<List<string>>
                {
                    Success = success,
                    Message = success ? "Pacientes importados com sucesso." : "Ocorreu um erro ao salvar os pacientes.",
                    StatusCode = success ? 200 : 500,
                    Data = invalidEntries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao importar pacientes: {ExceptionMessage}", ex.Message);
                return new ResultDataObject<List<string>>
                {
                    Success = false,
                    Data = invalidEntries,
                    Message = "Erro ao importar o arquivo CSV.",
                    StatusCode = 500,
                };
            }
        }

        /// <summary>
        /// Gera o conteúdo CSV para exportação de uma lista de pacientes.
        /// </summary>
        /// <param name="patients">Lista de pacientes a serem exportados.</param>
        /// <returns>Arquivo CSV em formato de array de bytes.</returns>
        private byte[] GenerateCsvContent(IEnumerable<Patient> patients)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Nome,CPF,DataDeNascimento,Email,Celular,Observacoes");

            foreach (var patient in patients)
            {
                csv.AppendLine($"{patient.Name},{patient.CPF},{patient.BirthDay},{patient.Mail},{patient.PhoneNumber},{patient.Observations}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }
}
