using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PageBusiness : IPageBusiness
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public PageBusiness(IUnitOfWork repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultObject> CreatePage(PageDTO pageDTO)
        {
            var page = _mapper.Map<Page>(pageDTO);

            var existingPage = await _repository.Pages.FirstOrDefault(p => p.Name == pageDTO.Name);
            if (existingPage != null)
            {
                return new ResultObject
                {
                    Message = "Uma página com o mesmo nome já existe.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Pages.Add(page);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Página criada com sucesso." : "Erro ao criar a página.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<Page>>> GetAllPages()
        {
            var pages = await _repository.Pages.GetAll();
            var result = _mapper.Map<List<Page>>(pages);

            bool success = result.Any();
            return new ResultDataObject<List<Page>>
            {
                Message = success ? "Páginas recuperadas com sucesso." : "Nenhuma página encontrada.",
                Data = result,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        public async Task<ResultObject> DeletePage(int id)
        {
            var page = await _repository.Pages.FirstOrDefault(p => p.Id == id);
            if (page == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Página não encontrada.",
                    Success = false
                };
            }

            _repository.Pages.Remove(page);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Página excluída com sucesso." : "Erro ao excluir a página.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}
