using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EFarma.Business
{
    public class StockRoomBusiness : IStockRoomBusiness
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public StockRoomBusiness(IUnitOfWork repository, IMapper mapper, HttpClient httpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<ResultObject> CreateStockRoom(StockRoomDTO stockRoomDTO)
        {
            var stockRoom = _mapper.Map<StockRoom>(stockRoomDTO);

            var existingStockRoom = await _repository.StockRooms.FirstOrDefault(sr => sr.Name == stockRoomDTO.Name);
            if (existingStockRoom != null)
            {
                return new ResultObject
                {
                    Message = "Uma Sala de Estoque com o mesmo nome já existe.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.StockRooms.Add(stockRoom);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Sala de Estoque criada com sucesso." : "Erro ao criar a Sala de Estoque.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<StockRoom>>> GetAllStockRooms()
        {
            var stockRooms = await _repository.StockRooms.GetAll();
            var result = _mapper.Map<List<StockRoom>>(stockRooms);

            bool success = result.Any();
            return new ResultDataObject<List<StockRoom>>
            {
                Message = success ? "Salas de Estoque recuperadas com sucesso." : "Nenhuma Sala de Estoque encontrada.",
                Data = result,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        public async Task<ResultObject> DeleteStockRoom(int id)
        {
            var stockRoom = await _repository.StockRooms.FirstOrDefault(sr => sr.Id == id);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Sala de Estoque não encontrada.",
                    Success = false
                };
            }

            _repository.StockRooms.Remove(stockRoom);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Sala de Estoque excluída com sucesso." : "Erro ao excluir a Sala de Estoque.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<StockRoom?>> GetStockRoom(int id)
        {
            var stockRoom = await _repository.StockRooms.GetStockRoomDetailed(id);
            bool success = stockRoom != null;

            return new()
            {
                Message = success ? "Sala de Estoque encontrada." : "Nenhuma Sala de Estoque encontrada.",
                Data = stockRoom,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultObject> InsertItemToStock(InStockItem inStockItem, int quantity)
        {
            var stockRoom = await _repository.StockRooms.FirstOrDefault(sr => sr.Id == inStockItem.StockRoomId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Sala de Estoque não encontrada.",
                    Success = false
                };
            }
            inStockItem.StockRoom = stockRoom;

            var medicament = await _repository.Medicaments.FirstOrDefault(m => m.Id == inStockItem.MedicamentId);
            if (medicament == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Medicamento não encontrado.",
                    Success = false
                };
            }
            inStockItem.Medicament = medicament;

            var newTagCodes = await GetNewTagCodes(inStockItem.StockRoom.UniqueId);

            if (quantity != newTagCodes.Count())
            {
                return new ResultObject
                {
                    StatusCode = 400,
                    Message = "A quantidade de medicamentos indicada não corresponde aos lidos nas prateleiras.",
                    Success = false
                };
            }

            var stockItemsToAdd = new List<InStockItem>();
            foreach (var tagCode in newTagCodes)
            {
                var stockItem = inStockItem.Clone();
                stockItem.TagCode = tagCode;
                stockItemsToAdd.Add(stockItem);
            }

            _repository.InStockItems.AddRange(stockItemsToAdd);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Item adicionado ao estoque com sucesso." : "Erro ao adicionar o item.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> EntryStockRoom(EntryLogDTO entryLogDTO)
        {
            var accessLog = _mapper.Map<AccessLog>(entryLogDTO);
            var employee = await _repository.Employees.GetEmployeeByTagCode(entryLogDTO.TagCode);
            if (employee == null)
            {
                return new ResultObject
                {
                    Message = "Tag não cadastrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.Employee = employee;

            var stockRoom = await _repository.StockRooms.FirstOrDefault(s => s.UniqueId == entryLogDTO.StockRoomUniqueId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    Message = "Sala não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.StockRoom = stockRoom;

            var hasAccess = employee.Role.Permissions
                .SelectMany(p => p.StockRooms)
                .Any(sr => sr == stockRoom);

            string[] splittedName = employee.Name.Split(' ');
            string formattedName = $"{splittedName.First()} {splittedName.Last()}";

            bool isUserAlreadyInside = hasAccess && await IsUserAlreadyOnStockRoom(employee.Id, stockRoom.Id);

            if (hasAccess)
            {
                if (isUserAlreadyInside)
                {
                    accessLog.Message = "Cartão foi lido com o funcionário já dentro da sala.";
                    accessLog.IsEntry = null;
                }
                else
                {
                    accessLog.Message = "Funcionário entrou na sala.";
                    accessLog.IsEntry = true;
                }
            }
            else
            {
                accessLog.Message = "Funcionário tentou acessar a sala, porém não possui acesso.";
                accessLog.IsEntry = null;
            }

            string responseMessage = hasAccess
                ? (isUserAlreadyInside ? $"{formattedName} já está dentro da sala." : $"Acesso liberado à {formattedName}.")
                : $"Acesso negado à {formattedName}. Favor contate o RH.";

            _repository.AccessLogs.Add(accessLog);
            await _repository.SaveChangesAsync();

            return new ResultObject
            {
                Message = responseMessage,
                StatusCode = hasAccess && !isUserAlreadyInside ? 200 : 403,
                Success = hasAccess && !isUserAlreadyInside
            };
        }

        public async Task<ResultObject> ExitStockRoom(string stockRoomUniqueId)
        {
            var entryAccessLog = await _repository.AccessLogs.GetFirstUnmatchedEntry(stockRoomUniqueId);
            if (entryAccessLog == null)
            {
                return new ResultObject
                {
                    Message = "Nenhuma entrada foi identificada na sala.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var employee = entryAccessLog.Employee;
            var stockRoom = entryAccessLog.StockRoom;

            var hasPendencies = await HasPendentPrescriptions(employee);
            if (hasPendencies)
            {
                var prescriptions = await _repository.Prescriptions.GetPendentPrescriptionsByTakeOutResponsibleId(employee.Id);
                await NotifyPendentPrescriptions(employee, stockRoom, prescriptions);

                return new ResultObject
                {
                    Message = "Saída bloqueada devido a prescrições pendentes.",
                    StatusCode = 403,
                    Success = false
                };
            }

            var newAccessLog = entryAccessLog.Clone();
            newAccessLog.Id = 0;
            newAccessLog.Date = DateTime.Now;

            var medicamentHasBeenTaken = await AnyMedicamentHasBeenTaken(stockRoom.UniqueId, stockRoom.Id);
            if (medicamentHasBeenTaken)
            {
                newAccessLog.Message = "Tentativa de saída bloqueada. Medicamentos faltantes no armário.";
                newAccessLog.IsEntry = null;
                _repository.AccessLogs.Add(newAccessLog);
                await _repository.SaveChangesAsync();

                await NotifyPendentPrescriptions(employee, stockRoom, new List<Prescription>());

                return new ResultObject
                {
                    Message = "Saída bloqueada. Medicamentos faltantes no armário.",
                    StatusCode = 403,
                    Success = false
                };
            }

            newAccessLog.Message = "Funcionário saiu da sala.";
            newAccessLog.IsEntry = false;

            var lastExit = await _repository.AccessLogs.GetDetailedLastExitByStockRoomUniqueId(stockRoomUniqueId);
            if (lastExit != null && lastExit.Date > entryAccessLog.Date)
            {
                newAccessLog.Message = "Funcionário saiu da sala. (Havia mais de um funcionário na sala.)";
            }

            _repository.AccessLogs.Add(newAccessLog);
            var success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Log de saída registrado com sucesso." : "Log de saída falhou ao ser armazenado.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<InStockItemView>>> GetAvailableMedicaments()
        {
            var inStockItems = await _repository.InStockItems.GetDetailedStockItems();
            var medicamentsDto = _mapper.Map<List<InStockItemView>>(inStockItems);

            var groupedItems = medicamentsDto
                .GroupBy(item => new { item.MedicamentId, item.StockRoomId })
                .Select(group => new InStockItemView
                {
                    MedicamentId = group.Key.MedicamentId,
                    StockRoomId = group.Key.StockRoomId,
                    StockRoomName = group.First().StockRoomName,
                    MedicamentName = group.First().MedicamentName,
                    MedicamentDosage = group.First().MedicamentDosage,
                    Quantity = group.Count()
                })
                .ToList();

            bool hasAnyItems = groupedItems.Count > 0;

            return new()
            {
                Message = hasAnyItems ? "Medicamentos encontrados." : "Nenhum medicamento encontrado.",
                Data = groupedItems,
                Success = hasAnyItems,
                StatusCode = hasAnyItems ? 200 : 404
            };
        }

        public async Task<ResultObject> CorrectAccess(EntryLogDTO entryLogDTO)
        {
            var accessLog = _mapper.Map<AccessLog>(entryLogDTO);
            var employee = await _repository.Employees.GetEmployeeByTagCode(entryLogDTO.TagCode);
            if (employee == null)
            {
                return new ResultObject
                {
                    Message = "Tag não cadastrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.Employee = employee;

            var stockRoom = await _repository.StockRooms.FirstOrDefault(s => s.UniqueId == entryLogDTO.StockRoomUniqueId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    Message = "Sala não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.StockRoom = stockRoom;

            var hasAccess = employee.Role.Permissions
                .SelectMany(p => p.StockRooms)
                .Any(sr => sr == stockRoom);

            string[] splittedName = employee.Name.Split(' ');
            string formattedName = $"{splittedName.First()} {splittedName.Last()}";

            bool isUserAlreadyInside = hasAccess && await IsUserAlreadyOnStockRoom(employee.Id, stockRoom.Id);

            if(!isUserAlreadyInside)
            {
                return new ResultObject
                {
                    Message = "Usuário não tem entradas pendentes.",
                    StatusCode = 403,
                    Success = false
                };
            }

            accessLog.Message = "Corrigido o acesso do funcionário à sala.";
            accessLog.IsEntry = false;

            _repository.AccessLogs.Add(accessLog);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Acesso corrigido com sucesso." : "Erro ao salvar correção de acesso.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        private async Task<List<string>> GetNewTagCodes(string uniqueId)
        {
            var readTagCodes = await GetReadTagCodes(uniqueId);
            var unassignedTags = new List<string>();

            foreach (var readTagCode in readTagCodes)
            {
                var existentItemOnStock = await _repository.InStockItems.FirstOrDefault(i => i.TagCode == readTagCode);
                if (existentItemOnStock == null)
                {
                    unassignedTags.Add(readTagCode);
                }
            }

            return unassignedTags;
        }

        private async Task<List<string>> GetReadTagCodes(string uniqueId)
        {
            try
            {
                var requestUrl = $"http://127.0.0.1:5000/TagCodes?code={uniqueId}";

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

        private async Task<bool> HasPendentPrescriptions(Employee employee)
        {
            var prescriptions = await _repository.Prescriptions.GetPendentPrescriptionsByTakeOutResponsibleId(employee.Id);
            return prescriptions.Count > 0;
        }

        private async Task<bool> IsUserAlreadyOnStockRoom(int employeeId, int stockRoomId)
        {
            var logs = await _repository.AccessLogs.GetLogsByEmployeeAndStockRoom(employeeId, stockRoomId);
            if (logs.Count == 0)
            {
                return false;
            }

            var entries = logs.Where(l => l.IsEntry.HasValue && l.IsEntry.Value).ToList();
            var exits = logs.Where(l => l.IsEntry.HasValue && !l.IsEntry.Value).ToList();

            return entries.Count != exits.Count;
        }

        private async Task NotifyPendentPrescriptions(Employee employee, StockRoom stockRoom, List<Prescription> prescriptions)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Uma retirada indevida foi feita pelo seguinte funcionário:");
            messageBuilder.AppendLine($"Nome: {employee.Name}");
            messageBuilder.AppendLine($"Email: {employee.Mail}");
            messageBuilder.AppendLine($"Telefone: {employee.Phone}");

            if (!string.IsNullOrEmpty(employee.EmployeeId))
            {
                messageBuilder.AppendLine($"Id de funcionário: {employee.EmployeeId}");
            }

            messageBuilder.AppendLine($"Sala de estoque:");
            messageBuilder.AppendLine($"Nome: {stockRoom.Name}");
            messageBuilder.AppendLine($"Endereço: {stockRoom.Address}");

            if(prescriptions.Count > 0)
                messageBuilder.AppendLine("Receita(s):");
            foreach (var prescription in prescriptions)
            {
                messageBuilder.AppendLine($"Id: {prescription.Id}");
                messageBuilder.AppendLine($"Data de Criação: {prescription.Date}");
                messageBuilder.AppendLine("Itens:");

                foreach (var item in prescription.Items)
                {
                    string medicamentName = $"{item.Medicament.Description} {item.Medicament.Dosage}{item.Medicament.Measure}";
                    messageBuilder.AppendLine($"   {medicamentName}");
                }
            }

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress("efarma@avisos.com");
                mail.To.Add(employee.Mail);
                mail.To.Add(employee.ResponsibleMail);
                mail.Subject = $"Retirada indevida da sala de estoque: {stockRoom.Name}";
                mail.Body = messageBuilder.ToString();

                using (var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your_email@gmail.com", "your_password"),
                    EnableSsl = true,
                })
                {
                    int retries = 3;
                    while (retries > 0)
                    {
                        try
                        {
                            await smtp.SendMailAsync(mail);
                            break;
                        }
                        catch (SmtpException ex)
                        {
                            retries--;
                            if (retries == 0)
                            {
                                Console.WriteLine($"Failed to send email: {ex.Message}");
                                throw;
                            }
                        }
                    }
                }
            }
        }

        private async Task<bool> AnyMedicamentHasBeenTaken(string stockRoomUniqueId, int stockRoomId)
        {
            var actualTagCodes = await GetReadTagCodes(stockRoomUniqueId);
            var actualItemsOnStock = await _repository.InStockItems.GetStockItemsByTagCodes(stockRoomId, actualTagCodes);
            var allItemsOnStock = await _repository.InStockItems.GetAll();

            var itemsTaken = allItemsOnStock.Except(actualItemsOnStock).ToList();

            return itemsTaken.Count > 0;
        }
    }
}
