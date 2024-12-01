using AutoMapper;
using EFarma.Business.Interfaces;
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
    /// Classe responsável pela lógica de negócios relacionada às Salas de Estoque (StockRooms) no sistema.
    /// Oferece funcionalidades para gerenciamento de salas de estoque, incluindo criação, exclusão, controle de acessos,
    /// inserção de itens em estoque e verificação de medicamentos disponíveis.
    /// </summary>
    public class StockRoomBusiness : IStockRoomBusiness
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="StockRoomBusiness"/>.
        /// </summary>
        /// <param name="repository">Instância do repositório para acesso ao banco de dados.</param>
        /// <param name="mapper">Instância do AutoMapper para conversão de modelos.</param>
        /// <param name="httpClient">Cliente HTTP para comunicação externa, como chamadas MQTT.</param>
        public StockRoomBusiness(IUnitOfWork repository, IMapper mapper, HttpClient httpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Cria uma nova Sala de Estoque (StockRoom).
        /// Verifica se já existe uma sala com o mesmo nome antes de realizar a criação.
        /// </summary>
        /// <param name="stockRoomDTO">Objeto <see cref="StockRoomDTO"/> contendo as informações da sala de estoque.</param>
        /// <returns>
        /// <see cref="ResultObject"/> indicando o status da operação.
        /// </returns>
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

        /// <summary>
        /// Obtém todas as salas de estoque registradas no sistema.
        /// </summary>
        /// <returns>
        /// <see cref="List{StockRoom}"/> contendo:
        /// - Data: Lista de salas de estoque.
        /// - Código 200: Salas encontradas.
        /// - Código 404: Nenhuma sala encontrada.
        /// </returns>
        public async Task<ResultDataObject<List<StockRoom>>> GetAllStockRooms()
        {
            var stockRooms = await _repository.StockRooms.GetAll();
            var result = _mapper.Map<List<StockRoom>>(stockRooms);

            bool success = result.Count != 0;
            return new ResultDataObject<List<StockRoom>>
            {
                Message = success ? "Salas de Estoque recuperadas com sucesso." : "Nenhuma Sala de Estoque encontrada.",
                Data = result,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        /// <summary>
        /// Deleta uma Sala de Estoque existente.
        /// </summary>
        /// <param name="id">ID da sala de estoque a ser deletada.</param>
        /// <returns>
        /// <see cref="ResultObject"/> indicando o status da operação.
        /// - Código 200: Sala deletada com sucesso.
        /// - Código 404: Sala não encontrada.
        /// - Código 500: Erro interno durante a exclusão.
        /// </returns>
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

        /// <summary>
        /// Obtém detalhes de uma Sala de Estoque específica.
        /// </summary>
        /// <param name="id">ID da sala de estoque.</param>
        /// <returns>
        /// <see cref="StockRoom"/> contendo os detalhes da sala.
        /// - Código 200: Sala encontrada.
        /// - Código 404: Sala não encontrada.
        /// </returns>
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

        /// <summary>
        /// Adiciona itens ao estoque de uma sala específica.
        /// Valida a existência da sala e dos medicamentos antes de realizar a inserção.
        /// </summary>
        /// <param name="inStockItem">Objeto representando o item a ser inserido.</param>
        /// <param name="quantity">Quantidade de itens a serem adicionados.</param>
        /// <param name="employeeId">ID do funcionário responsável pela operação.</param>
        /// <returns>
        /// <see cref="ResultObject"/> indicando o status da operação.
        /// </returns>
        public async Task<ResultObject> InsertItemToStock(InStockItem inStockItem, int quantity, int employeeId)
        {
            var lastEntryAccessLog = await _repository.AccessLogs.GetLastUnmatchedEntry(employeeId);
            if (lastEntryAccessLog == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Usuário não está dentro de nenhuma sala de estoque.",
                    Success = false
                };
            }
            inStockItem.StockRoom = lastEntryAccessLog.StockRoom;

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

        /// <summary>
        /// Registra a entrada de um funcionário em uma Sala de Estoque.
        /// Verifica permissões de acesso antes de registrar a entrada.
        /// </summary>
        /// <param name="entryLogDTO">Objeto contendo os dados de acesso do funcionário.</param>
        /// <returns>
        /// <see cref="ResultObject"/> indicando o status da operação.
        /// - Código 200: Entrada registrada com sucesso.
        /// - Código 403: Acesso negado.
        /// - Código 404: Funcionário ou sala não encontrado.
        /// - Código 500: Erro interno durante a operação.
        /// </returns>
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

        /// <summary>
        /// Registra a saída de um funcionário de uma Sala de Estoque.
        /// Verifica inconsistências como medicamentos faltantes ou prescrições pendentes.
        /// </summary>
        /// <param name="entryLogDTO">Objeto contendo os dados de saída do funcionário.</param>
        /// <returns>
        /// <see cref="ResultObject"/> indicando o status da operação.
        /// - Código 200: Saída registrada com sucesso.
        /// - Código 404: Funcionário ou sala não encontrado.
        /// - Código 500: Erro interno durante a operação.
        /// </returns>
        public async Task<ResultObject> ExitStockRoom(EntryLogDTO entryLogDTO)
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

            var entryAccessLog = await _repository.AccessLogs.GetFirstUnmatchedEntry(employee.Id);

            if(entryAccessLog == null)
            {
                return new ResultObject
                {
                    Message = "Entrada não registrada para esse usuário.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var newAccessLog = entryAccessLog.Clone();
            newAccessLog.Id = 0;
            newAccessLog.Date = DateTime.Now;
            newAccessLog.IsEntry = false;
            newAccessLog.Message = "Funcionário saiu da sala.";

            var medicamentHasBeenTaken = await AnyMedicamentHasBeenTaken(stockRoom.UniqueId, stockRoom.Id);
            if (medicamentHasBeenTaken.Count > 0)
            {
                _repository.InStockItems.RemoveRange(medicamentHasBeenTaken);

                newAccessLog.Message = "Saída indevida. Haviam medicamentos faltantes no armário.";
                var combinedDetails = string.Join("; ", medicamentHasBeenTaken.Select(item => item.ToString()));

                newAccessLog.Detail = combinedDetails;

                _repository.AccessLogs.Add(newAccessLog);
                await _repository.SaveChangesAsync();

                await MailManager.NotifyMedicamentsTaken(employee, stockRoom, medicamentHasBeenTaken);

                return new ResultObject
                {
                    Message = "Saída indevida. Medicamentos faltantes no armário.",
                    StatusCode = 200,
                    Success = true
                };
            }

            var pendentPrescriptions = await _repository.Prescriptions.GetUnresolvedPrescriptionsByTakeOutResponsibleId(employee.Id);
            if (pendentPrescriptions.Count > 0)
            {
                await MailManager.NotifyUnresolvedPrescriptions(employee, stockRoom, pendentPrescriptions);

                foreach(var prescription in pendentPrescriptions)
                {
                    prescription.Status = Prescription.PendentMessage;
                    _repository.Prescriptions.Update(prescription);
                }

                newAccessLog.Message = "Saída indevida. Haviam prescrições em aberto.";
                var combinedDetails = string.Join("; ", pendentPrescriptions.Select(item => item.ToString()));

                newAccessLog.Detail = combinedDetails;

                _repository.AccessLogs.Add(newAccessLog);
                await _repository.SaveChangesAsync();

                return new ResultObject
                {
                    Message = "Saída indevida. Haviam prescrições em aberto.",
                    StatusCode = 200,
                    Success = true
                };
            }

            var lastExit = await _repository.AccessLogs.GetDetailedLastExitByStockRoomUniqueId(stockRoom.UniqueId);
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

        /// <summary>
        /// Recupera a lista de medicamentos disponíveis no estoque.
        /// Opcionalmente filtra os resultados pelo nome do medicamento e agrupa-os por sala de estoque e medicamento.
        /// </summary>
        /// <param name="medicamentName">Nome opcional do medicamento para filtrar os resultados. Caso seja nulo, todos os medicamentos serão retornados.</param>
        /// <returns>
        /// <see cref="List{InStockItemView}"/> contendo:
        /// - A lista agrupada de medicamentos disponíveis no estoque.
        /// - Status 200 se medicamentos forem encontrados.
        /// - Status 404 se nenhum medicamento for encontrado.
        /// </returns>
        public async Task<ResultDataObject<List<InStockItemView>>> GetAvailableMedicaments(string? medicamentName)
        {
            var inStockItems = await _repository.InStockItems.GetDetailedStockItems(medicamentName);
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

        /// <summary>
        /// Corrige o registro de acesso de um funcionário a uma sala de estoque.
        /// Verifica se o funcionário possui entradas pendentes na sala e valida as permissões de acesso.
        /// </summary>
        /// <param name="entryLogDTO">
        /// Objeto contendo o código de identificação do funcionário (tag) e o identificador único da sala de estoque.
        /// </param>
        /// <returns>
        /// <see cref="ResultObject"/> indicando o resultado da correção.
        /// - Status 200 se a correção for bem-sucedida.
        /// - Status 403 se o funcionário não possuir entradas pendentes.
        /// - Status 404 se o funcionário ou a sala de estoque não forem encontrados.
        /// - Status 500 em caso de erro interno durante a operação.
        /// </returns>
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

        /// <summary>
        /// Obtém a lista de novos códigos de etiquetas detectados pelo sistema embarcado em uma sala de estoque específica.
        /// Filtra os códigos de etiquetas que já estão atribuídos a itens em estoque.
        /// </summary>
        /// <param name="uniqueId">Identificador único da sala de estoque.</param>
        /// <returns>Uma lista de novos códigos de etiquetas não atribuídos detectados na sala de estoque.</returns>
        private async Task<List<string>> GetNewTagCodes(string uniqueId)
        {
            var readTagCodes = await MqttRequest.GetReadTagCodes(_httpClient, uniqueId);
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

        /// <summary>
        /// Verifica se um funcionário possui prescrições pendentes atribuídas a ele.
        /// </summary>
        /// <param name="employee">O funcionário cujas prescrições estão sendo verificadas.</param>
        /// <returns>
        /// Um booleano indicando se o funcionário possui prescrições pendentes.
        /// </returns>
        private async Task<bool> HasPendentPrescriptions(Employee employee)
        {
            var prescriptions = await _repository.Prescriptions.GetUnresolvedPrescriptionsByTakeOutResponsibleId(employee.Id);
            return prescriptions.Count > 0;
        }

        /// <summary>
        /// Determina se um funcionário ainda está dentro de uma sala de estoque com base nos registros de acesso.
        /// Compara o número de registros de entrada e saída do funcionário na sala de estoque especificada.
        /// </summary>
        /// <param name="employeeId">ID do funcionário.</param>
        /// <param name="stockRoomId">ID da sala de estoque.</param>
        /// <returns>
        /// Um booleano indicando se o funcionário ainda está na sala de estoque.
        /// </returns>
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

        /// <summary>
        /// Verifica se algum medicamento foi retirado de uma sala de estoque.
        /// Compara os códigos de etiquetas atuais lidos pelo sistema embarcado com os itens atualmente em estoque.
        /// </summary>
        /// <param name="stockRoomUniqueId">Identificador único da sala de estoque.</param>
        /// <param name="stockRoomId">ID da sala de estoque.</param>
        /// <returns>
        /// Uma lista de medicamentos que foram retirados da sala de estoque.
        /// Caso nenhum item tenha sido retirado, retorna uma lista vazia.
        /// </returns>
        private async Task<List<InStockItem>?> AnyMedicamentHasBeenTaken(string stockRoomUniqueId, int stockRoomId)
        {
            var actualTagCodes = await MqttRequest.GetReadTagCodes(_httpClient, stockRoomUniqueId);
            var actualItemsOnStock = await _repository.InStockItems.GetStockItemsByTagCodes(stockRoomId, actualTagCodes);
            var allItemsOnStock = await _repository.InStockItems.GetAllDetailed();

            var itemsTaken = allItemsOnStock.Except(actualItemsOnStock).ToList();

            if (itemsTaken.Count > 0)
            {
                _repository.InStockItems.RemoveRange(itemsTaken);
            }

            return itemsTaken;
        }

        /// <summary>
        /// Remove acentos e diacríticos de uma string de texto.
        /// Este método é utilizado para garantir compatibilidade com sistemas que não suportam caracteres acentuados.
        /// </summary>
        /// <param name="text">A string de texto a ser processada.</param>
        /// <returns>A string de texto processada, sem acentos ou diacríticos.</returns>
        public static string RemoveAcentuation(string text)
        {
            return
                System.Web.HttpUtility.UrlDecode(
                    System.Web.HttpUtility.UrlEncode(
                        text, Encoding.GetEncoding("iso-8859-7")));
        }
    }
}
