using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    /// <summary>
    /// Classe responsável pelas operações relacionadas a medicamentos.
    /// </summary>
    public class MedicamentBusiness : IMedicamentBusiness
    {
        private readonly ILogger<Medicament> _logger;
        private readonly IUnitOfWork _repository;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="MedicamentBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="repository">Instância do repositório para manipulação de dados de medicamentos.</param>
        public MedicamentBusiness(ILogger<Medicament> logger, IUnitOfWork repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Cria um novo medicamento no sistema, verificando previamente se ele já existe com a mesma descrição, dosagem e medida.
        /// </summary>
        /// <param name="medicament">Objeto do medicamento a ser criado.</param>
        /// <returns>Resultado da operação de criação com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> Create(Medicament medicament)
        {
            var existent_medicament = await _repository.Medicaments.FirstOrDefault(m =>
                m.Description == medicament.Description &&
                m.Dosage == medicament.Dosage &&
                m.Measure == medicament.Measure
            );

            if (existent_medicament != null)
            {
                return new ResultObject
                {
                    Message = "Este medicamento já existe no sistema.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Medicaments.Add(medicament);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Medicamento criado com sucesso." : "Ocorreu um erro ao criar o medicamento.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Obtém todos os medicamentos cadastrados no sistema.
        /// </summary>
        /// <returns>Objeto de resultado contendo a lista de medicamentos, o status da operação e uma mensagem apropriada.</returns>
        public async Task<ResultDataObject<List<Medicament>>> GetAll()
        {
            var medicaments = await _repository.Medicaments.GetAll();

            bool success = medicaments.Count != 0;

            return new()
            {
                Message = success ? "Medicamentos encontrados." : "Nenhum medicamento encontrado.",
                Data = medicaments,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }

        /// <summary>
        /// Atualiza os dados de um medicamento existente no sistema.
        /// </summary>
        /// <param name="id">ID do medicamento a ser atualizado.</param>
        /// <param name="medicamentDto">Objeto contendo os novos dados do medicamento.</param>
        /// <returns>Resultado da operação de atualização com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> Update(int id, MedicamentDTO medicamentDto)
        {
            var existingMedicament = await _repository.Medicaments.FirstOrDefault(p => p.Id == id);
            if (existingMedicament == null)
            {
                return new ResultObject
                {
                    Message = "Medicamento não foi encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            existingMedicament.Description = medicamentDto.Description;
            existingMedicament.Dosage = medicamentDto.Dosage;
            existingMedicament.Measure = medicamentDto.Measure;

            _repository.Medicaments.Update(existingMedicament);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Medicamento atualizado com sucesso." : "Um erro ocorreu durante a atualização do medicamento.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Exclui um medicamento do sistema pelo ID especificado.
        /// </summary>
        /// <param name="id">ID do medicamento a ser excluído.</param>
        /// <returns>Resultado da operação de exclusão com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> Delete(int id)
        {
            var medicament = await _repository.Medicaments.FirstOrDefault(e => e.Id == id);
            if (medicament == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Medicamento não encontrado.",
                    Success = false
                };
            }

            _repository.Medicaments.Remove(medicament);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Medicamento excluído com sucesso." : "Ocorreu um erro ao excluir o medicamento.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}
