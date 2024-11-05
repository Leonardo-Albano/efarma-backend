using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;

namespace EFarma.Business
{
    public class PrescriptionBusiness : IPrescriptionBusiness
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public PrescriptionBusiness(ILogger<PrescriptionController> logger, IUnitOfWork repository, IMapper mapper, HttpClient httpClient)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _httpClient = httpClient;
        }

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
            return new ResultObject
            {
                Message = success ? "Receita criada com sucesso." : "Ocorreu um erro ao criar a receita.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

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

        public async Task<ResultDataObject<List<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date)
        {
            var detailedPrescriptions = await _repository.Prescriptions.GetDetailedPrescriptions(cpf, date);
            var prescriptions = _mapper.Map<List<PrescriptionView>>(detailedPrescriptions);

            bool success = prescriptions.Any();

            return new()
            {
                Message = success ? "Receitas encontradas." : "Nenhuma receita encontrada.",
                Data = prescriptions,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

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

        public async Task<ResultObject> RemoveItemsFromStock(RemovePrescriptionItemsDTO prescriptionItemsDTO)
        {
            var prescription = await _repository.Prescriptions.GetDetailedPrescriptionById(prescriptionItemsDTO.PrescriptionId);
            if (prescription == null)
            {
                return new ResultObject
                {
                    Message = "Receita não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var stockRoom = await _repository.StockRooms.FirstOrDefault(p => p.Id == prescriptionItemsDTO.StockRoomId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    Message = "Sala de estoque não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var responsible = await _repository.Employees.FirstOrDefault(p => p.Id == prescriptionItemsDTO.TakeOutResponsibleId);
            if (responsible == null)
            {
                return new ResultObject
                {
                    Message = "Funcionário não foi encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var log = new AccessLog()
            {
                Employee = responsible,
                IsEntry = false,
                StockRoom = stockRoom,
                Date = DateTime.Now
            };

            prescription.TakeOutResponsible = responsible;

            var kvResult = await CompareWithActualMedicamentsAtStock(
                prescription.Items
                    .SelectMany(i => Enumerable.Repeat(i.Medicament, i.PrescribedQuantity))
                    .ToList(),
                stockRoom.UniqueId, stockRoom.Id
            );

            bool success = kvResult.Key;
            string message = kvResult.Value;
            bool storeSuccess = await _repository.SaveChangesAsync() > 0;

            log.Message = message;
            prescription.Status = success ? Prescription.ConcludedMessage : Prescription.PendentMessage;

            _repository.AccessLogs.Add(log);
            _repository.Prescriptions.Update(prescription);

            int statusCode = !storeSuccess ? 500 : (success ? 200 : 403);

            return new()
            {
                Message = message,
                StatusCode = statusCode,
                Success = success || storeSuccess // Success is true if either is true
            };
        }

        private async Task<List<string>> GetReadTagCodes(string uniqueId)
        {
            try
            {
                var requestUrl = $"http://http://157.230.224.194:8501/TagCodes?code={uniqueId}";

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var tagCodeResponse = JsonConvert.DeserializeObject<List<string>>(responseBody);

                return tagCodeResponse ?? [];
            }
            catch (Exception ex)
            {
                return [];
            }
        }

        private async Task<KeyValuePair<bool, string>> CompareWithActualMedicamentsAtStock(List<Medicament> prescriptionMedicaments, string stockRoomUniqueId, int stockRoomId)
        {
            var actualTagCodes = await GetReadTagCodes(stockRoomUniqueId);
            var actualItemsOnStock = await _repository.InStockItems.GetStockItemsByTagCodes(stockRoomId, actualTagCodes);
            var allItemsOnStock = await _repository.InStockItems.GetAll();

            var itemsTaken = allItemsOnStock.Except(actualItemsOnStock).ToList();
            int extraItemsCount = 0;
            int missingItemsCount = 0;

            foreach (var itemOnStock in itemsTaken)
            {
                var equivalentItem = prescriptionMedicaments.FirstOrDefault(m => m.Id == itemOnStock.MedicamentId);

                if (equivalentItem == null)
                {
                    extraItemsCount++;
                }
                else
                {
                    prescriptionMedicaments.Remove(equivalentItem);
                    _repository.InStockItems.Remove(itemOnStock);
                }
            }

            missingItemsCount = prescriptionMedicaments.Count;

            if (extraItemsCount > 0)
            {
                return new(false, $"Os medicamentos retirados não estão de acordo com a receita. {extraItemsCount} medicamento(s) a mais.");
            }

            var message = "Medicamentos retirados de acordo com a receita. ";
            if (missingItemsCount > 0)
                message += $"{missingItemsCount} medicamento(s) a menos.";

            return new(true, message);
        }
    }
}
