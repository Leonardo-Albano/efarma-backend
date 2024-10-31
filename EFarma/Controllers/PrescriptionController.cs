using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IPrescriptionBusiness _business;
        private readonly IMapper _mapper;

        public PrescriptionController(ILogger<PrescriptionController> logger, IPrescriptionBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint para criar uma nova prescrição no sistema.
        /// Valida se os medicamentos, funcionário (com CRM registrado) e o paciente existem antes de realizar a criação.
        /// </summary>
        /// <param name="prescriptionDTO">Objeto <see cref="PrescriptionDTO"/> contendo as informações da prescrição a ser criada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Prescrição criada com sucesso.</response>
        /// <response code="403">Funcionário não tem permissão para prescrever (CRM não registrado).</response>
        /// <response code="404">Medicamento, funcionário ou paciente não encontrado.</response>
        /// <response code="500">Erro interno ao tentar criar a prescrição.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePrescription([FromBody] PrescriptionDTO prescriptionDTO)
        {
            var prescription = _mapper.Map<Prescription>(prescriptionDTO);
            var result = await _business.CreatePrescription(prescription);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para remover itens do estoque com base em uma prescrição.
        /// Valida se a prescrição e a sala de estoque existem e verifica se os medicamentos no estoque correspondem aos itens prescritos.
        /// Caso sejam retirados mais medicamentos do que os prescritos na receita, o sistema não permitirá.
        /// Caso sejam retirados menos medicamentos do que os prescritos na receita, o sistema permitirá.
        /// </summary>
        /// <param name="prescriptionItemsDTO">Objeto <see cref="RemovePrescriptionItemsDTO"/> contendo as informações da prescrição e da sala de estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Itens removidos do estoque com sucesso e prescrição finalizada.</response>
        /// <response code="403">Os medicamentos no estoque não correspondem aos itens prescritos.</response>
        /// <response code="404">Prescrição ou sala de estoque não encontrada.</response>
        /// <response code="500">Erro interno ao tentar remover os itens do estoque.</response>
        [HttpPost("RemoveItems")]
        public async Task<ActionResult<ResultObject>> RemoveItemsFromStock([FromBody] RemovePrescriptionItemsDTO prescriptionItemsDTO)
        {
            var result = await _business.RemoveItemsFromStock(prescriptionItemsDTO);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para obter uma lista de prescrições filtradas por CPF do paciente e/ou data da prescrição.
        /// </summary>
        /// <param name="cpf">CPF do paciente a ser consultado (opcional).</param>
        /// <param name="date">Data da prescrição a ser consultada (opcional).</param>
        /// <returns>Objeto <see cref="ResultDataObject{List{PrescriptionView}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Prescrições encontradas com sucesso.</response>
        /// <response code="404">Nenhuma prescrição encontrada com os critérios fornecidos.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<PrescriptionView>>>> GetPrescriptions(string? cpf, DateTime? date)
        {
            var result = await _business.GetPrescriptions(cpf, date);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter os itens de uma prescrição específica com base no ID da prescrição fornecido.
        /// </summary>
        /// <param name="prescriptionId">ID da prescrição cujos itens serão consultados.</param>
        /// <returns>Objeto <see cref="ResultDataObject{List{PrescriptionItemView}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Itens da prescrição encontrados com sucesso.</response>
        /// <response code="404">Nenhum item encontrado para a prescrição fornecida.</response>
        [HttpGet("{prescriptionId}")]
        public async Task<ActionResult<ResultDataObject<List<PrescriptionItemView>>>> GetPrescriptionItems(int prescriptionId)
        {
            var result = await _business.GetPrescriptionItems(prescriptionId);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para deletar uma prescrição existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da prescrição a ser deletada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Prescrição deletada com sucesso.</response>
        /// <response code="404">Prescrição não encontrada.</response>
        /// <response code="500">Erro interno ao tentar deletar a prescrição.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePrescription(int id)
        {
            var result = await _business.DeletePrescription(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

    }
}
