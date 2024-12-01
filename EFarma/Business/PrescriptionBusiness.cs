using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;
using EFarma.Utils;
using System.Text;

namespace EFarma.Business
{
    /// <summary>
    /// Classe de negócio responsável pelas operações relacionadas às receitas no sistema.
    /// </summary>
    public class PrescriptionBusiness : IPrescriptionBusiness
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="PrescriptionBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="repository">Unidade de trabalho para manipulação dos repositórios.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        /// <param name="httpClient">Cliente HTTP para comunicação com serviços externos.</param>
        public PrescriptionBusiness(ILogger<PrescriptionController> logger, IUnitOfWork repository, IMapper mapper, HttpClient httpClient)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Cria uma nova receita no sistema.
        /// Valida os medicamentos, o funcionário (com CRM registrado) e o paciente antes de criar a receita.
        /// </summary>
        /// <param name="prescription">Objeto contendo as informações da receita a ser criada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> contendo o status da operação e o código HTTP correspondente.</returns>
        public async Task<ResultObject> CreatePrescription(Prescription prescription)
        {
            prescription.Status = Prescription.CreatedMessage;

            foreach (var item in prescription.Items)
            {
                var medicament = await _repository.Medicaments.FirstOrDefault(m => m.Id == item.MedicamentId);
                if (medicament == null)
                {
                    return new ResultObject
                    {
                        Message = $"Medicamento com ID {item.MedicamentId} não está registrado.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                item.Prescription = prescription;
                item.Medicament = medicament;
            }

            var employee = await _repository.Employees.FirstOrDefault(m => m.Id == prescription.EmployeeId);
            if (employee == null)
            {
                return new ResultObject
                {
                    Message = "Funcionário não encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }
            else if (string.IsNullOrEmpty(employee.CRM))
            {
                return new ResultObject
                {
                    Message = "Funcionário não está autorizado a prescrever (CRM não registrado).",
                    StatusCode = 403,
                    Success = false
                };
            }
            prescription.Employee = employee;

            var patient = await _repository.Patients.FirstOrDefault(m => m.CPF == prescription.CPF);
            if (patient == null)
            {
                return new ResultObject
                {
                    Message = "Paciente não encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }
            prescription.Patient = patient;

            _repository.Prescriptions.Add(prescription);

            bool success = await _repository.SaveChangesAsync() > 0;

            string message = string.Empty;
            try
            {
                await MailManager.SendPrescriptionToPatient(prescription);

                message = "Receita criada e e-mail enviado com sucesso.";
            }
            catch (Exception ex)
            {
                message = "Receita criada, porém: " + ex.Message;
            }

            return new ResultObject
            {
                Message = success ? message : "Ocorreu um erro ao criar a receita.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Deleta uma receita existente no sistema.
        /// </summary>
        /// <param name="id">ID da receita a ser deletada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> contendo o status da operação e o código HTTP correspondente.</returns>
        public async Task<ResultObject> DeletePrescription(int id)
        {
            var prescription = await _repository.Prescriptions.FirstOrDefault(e => e.Id == id);
            if (prescription == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Receita não encontrada.",
                    Success = false
                };
            }

            _repository.Prescriptions.Remove(prescription);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Receita excluída com sucesso." : "Ocorreu um erro ao excluir a receita.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Obtém os itens de uma receita específica.
        /// </summary>
        /// <param name="prescriptionId">ID da receita cujos itens serão consultados.</param>
        /// <returns>Objeto <see cref="List{PrescriptionItemView}"/> contendo os itens da receita.</returns>
        public async Task<ResultDataObject<List<PrescriptionItemView>>> GetPrescriptionItems(int prescriptionId)
        {
            var items = await _repository.PrescriptionItems.GetPrescriptionItems(prescriptionId);

            var prescriptionItemsView = items.Select(item => new PrescriptionItemView
            {
                Name = item.Medicament.Description,
                Dosage = item.Medicament.Dosage,
                Measure = item.Medicament.Measure,
                Quantity = item.PrescribedQuantity
            }).ToList();

            bool has_items = prescriptionItemsView.Count != 0;
            return new()
            {
                Data = prescriptionItemsView,
                Message = has_items ? "Itens da receita encontrados." : "Nenhum item encontrado para esta receita.",
                StatusCode = has_items ? 200 : 404,
                Success = has_items
            };
        }

        /// <summary>
        /// Obtém uma lista de receitas filtradas por CPF do paciente, data ou status de pendência.
        /// </summary>
        /// <param name="cpf">CPF do paciente a ser filtrado (opcional).</param>
        /// <param name="date">Data da receita a ser filtrada (opcional).</param>
        /// <param name="filterPendent">Indica se apenas receitas pendentes devem ser retornadas.</param>
        /// <returns>Objeto <see cref="List{PrescriptionView}"/> contendo as receitas filtradas.</returns>
        public async Task<ResultDataObject<List<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date, bool filterPendent)
        {
            var detailedPrescriptions = await _repository.Prescriptions.GetDetailedPrescriptions(filterPendent, cpf, date);
            var prescriptions = _mapper.Map<List<PrescriptionView>>(detailedPrescriptions);

            bool success = prescriptions.Count != 0;

            return new()
            {
                Message = success ? "Receitas encontradas." : "Nenhuma receita encontrada.",
                Data = prescriptions,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        /// <summary>
        /// Obtém os detalhes de uma receita específica.
        /// </summary>
        /// <param name="id">ID da receita cujos detalhes serão obtidos.</param>
        /// <returns>Objeto <see cref="PrescriptionViewDetailed"/> contendo os detalhes da receita.</returns>
        public async Task<ResultDataObject<PrescriptionViewDetailed?>> GetPrescriptionDetailed(int id)
        {
            var detailedPrescription = await _repository.Prescriptions.GetDetailedPrescriptionById(id);
            if (detailedPrescription == null)
            {
                return new ResultDataObject<PrescriptionViewDetailed?>
                {
                    Data = null,
                    StatusCode = 404,
                    Message = "Receita não encontrada.",
                    Success = false
                };
            }

            var prescriptionView = _mapper.Map<PrescriptionViewDetailed>(detailedPrescription);

            bool success = prescriptionView != null;
            return new()
            {
                Message = success ? "Receita encontrada." : "Erro ao converter receita.",
                Data = prescriptionView,
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Realiza a retirada de medicamentos de uma receita específica em uma sala de estoque.
        /// Este método realiza diversas validações, incluindo a existência da receita, acesso à sala de estoque 
        /// e o responsável pela retirada. Compara os medicamentos efetivamente retirados com os itens da receita
        /// e atualiza o status da receita de acordo com o resultado.
        /// </summary>
        /// <param name="prescriptionItemsDTO">
        /// Objeto contendo os detalhes necessários para processar a retirada, incluindo:
        /// </param>
        /// <returns>
        /// Um <see cref="List{WithdrawItem}"/> contendo:
        /// - Data: Uma lista de discrepâncias (itens extras ou faltantes).
        /// - Message: Um resumo do resultado da operação.
        /// - StatusCode: Códigos de status HTTP-like indicando o resultado:
        ///   - 200: Retirada realizada com sucesso e compatível com a receita.
        ///   - 202: Receita já concluída.
        ///   - 403: Discrepâncias encontradas ou problemas de acesso.
        ///   - 404: Receita ou funcionário responsável não encontrados.
        ///   - 500: Erro interno ao salvar a operação.
        /// </returns>
        /// <remarks>
        /// O método realiza as seguintes etapas:
        /// 1. Valida a existência da receita.
        /// 2. Verifica se a receita já foi concluída.
        /// 3. Valida a existência do funcionário responsável.
        /// 4. Confirma se o funcionário tem acesso à sala de estoque relevante.
        /// 5. Compara os itens retirados com os itens da receita:
        ///    - Identifica itens extras retirados.
        ///    - Identifica itens faltantes.
        /// 6. Registra a operação e atualiza o status da receita:
        ///    - O status é definido como "Concluded" se a retirada for compatível com a receita.
        ///    - O status é definido como "Unresolved" se houver discrepâncias.
        /// 7. Salva os resultados no banco de dados, incluindo o registro de acesso e as atualizações da receita.
        /// </remarks>
        public async Task<ResultDataObject<List<WithdrawItem>>> WithdrawPrescription(RemovePrescriptionItemsDTO prescriptionItemsDTO)
        {
            var prescription = await _repository.Prescriptions.GetDetailedPrescriptionById(prescriptionItemsDTO.PrescriptionId);
            if (prescription == null)
            {
                return new ResultDataObject<List<WithdrawItem>>
                {
                    Data = [],
                    Message = "Receita não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            else if(prescription.Status == Prescription.ConcludedMessage)
            {
                return new ResultDataObject<List<WithdrawItem>>
                {
                    Data = [],
                    Message = "Receita foi encontrada, porém já está finalizada.",
                    StatusCode = 202,
                    Success = true
                };
            }

            var responsible = await _repository.Employees.FirstOrDefault(p => p.Id == prescriptionItemsDTO.TakeOutResponsibleId);
            if (responsible == null)
            {
                return new ResultDataObject<List<WithdrawItem>>
                {
                    Data = [],
                    Message = "Funcionário não foi encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var lastEmployeeAccessLog = await _repository.AccessLogs.GetLastUnmatchedEntry(prescriptionItemsDTO.TakeOutResponsibleId);
            if (lastEmployeeAccessLog == null)
            {
                return new ResultDataObject<List<WithdrawItem>>
                {
                    Data = [],
                    Message = "Usuário não está em nenhuma sala.",
                    StatusCode = 403,
                    Success = false
                };
            }
            var stockRoom = lastEmployeeAccessLog.StockRoom;

            var log = new AccessLog()
            {
                Employee = responsible,
                IsEntry = null,
                StockRoom = stockRoom,
                Date = DateTime.Now
            };

            prescription.TakeOutResponsible = responsible;

            var prescriptionMedicaments = prescription.Items.SelectMany(i => Enumerable.Repeat(i.Medicament, i.PrescribedQuantity)).ToList();
            var result = await CompareWithActualMedicamentsAtStock(prescriptionMedicaments, stockRoom.UniqueId, stockRoom.Id);

            log.Message = result.Message;
            log.Detail = string.Join("; ", result.Data.Select(item => item.ToString()));

            prescription.Status = result.Success ? Prescription.ConcludedMessage : Prescription.UnresolvedMessage;

            _repository.AccessLogs.Add(log);
            _repository.Prescriptions.Update(prescription);

            bool storeSuccess = await _repository.SaveChangesAsync() > 0;
            result.StatusCode = !storeSuccess ? 500 : result.StatusCode;
            result.Success = result.Success && storeSuccess;
            return result;
        }

        /// <summary>
        /// Compara os medicamentos prescritos com os disponíveis no estoque.
        /// </summary>
        /// <param name="prescriptionMedicaments">Lista de medicamentos prescritos.</param>
        /// <param name="stockRoomUniqueId">Identificador único da sala de estoque.</param>
        /// <param name="stockRoomId">ID da sala de estoque.</param>
        /// <returns>Objeto <see cref="List{WithdrawItem}"/> contendo o resultado da comparação.</returns>
        private async Task<ResultDataObject<List<WithdrawItem>>> CompareWithActualMedicamentsAtStock(List<Medicament> prescriptionMedicaments, string stockRoomUniqueId, int stockRoomId)
        {
            var actualTagCodes = await MqttRequest.GetReadTagCodes(_httpClient, stockRoomUniqueId);
            var actualItemsOnStock = await _repository.InStockItems.GetStockItemsByTagCodes(stockRoomId, actualTagCodes);
            var allItemsOnStock = await _repository.InStockItems.GetAllDetailed();

            var itemsTaken = allItemsOnStock.Except(actualItemsOnStock).ToList();
            int extraItemsCount = 0;
            List<WithdrawItem> extraMedicaments = [];
            List<InStockItem> correctMedicaments = [];
            int missingItemsCount = 0;

            foreach (var itemOnStock in itemsTaken)
            {
                var equivalentItem = prescriptionMedicaments.FirstOrDefault(m => m.Id == itemOnStock.MedicamentId);

                if (equivalentItem == null)
                {
                    extraItemsCount++;

                    var existingExtraMedicament = extraMedicaments
                    .FirstOrDefault(m =>
                        m.Dosage == itemOnStock.Medicament.Dosage &&
                        m.Measure == itemOnStock.Medicament.Measure &&
                        m.Name == itemOnStock.Medicament.Description);

                    if (existingExtraMedicament != null)
                    {
                        existingExtraMedicament.Message = $"Retirado(s) {++existingExtraMedicament.Quantity} medicamento(s) a mais";
                    }
                    else
                    {
                        extraMedicaments.Add(new WithdrawItem
                        {
                            Dosage = itemOnStock.Medicament.Dosage,
                            Measure = itemOnStock.Medicament.Measure,
                            Name = itemOnStock.Medicament.Description,
                            Message = "Retirado(s) 1 medicamento(s) a mais",
                            Quantity = 1
                        });
                    }
                }
                else
                {
                    prescriptionMedicaments.Remove(equivalentItem);
                    correctMedicaments.Add(itemOnStock);
                }
            }

            missingItemsCount = prescriptionMedicaments.Count;

            if (extraItemsCount > 0)
            {
                return new(){
                    Data = extraMedicaments,
                    Success = false,
                    Message = $"Os medicamentos retirados não estão de acordo com a receita. {extraItemsCount} medicamento(s) a mais.",
                    StatusCode = 403
                };
            }

            _repository.InStockItems.RemoveRange(correctMedicaments);
            var message = "Medicamentos retirados de acordo com a receita. ";
            if (missingItemsCount > 0)
                message += $"{missingItemsCount} medicamento(s) a menos.";

            return new(){
                Data = [],
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }
    }
}
