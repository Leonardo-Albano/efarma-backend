using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IPageBusiness
    {
        Task<ResultObject> CreatePage(PageDTO pageDTO);
        Task<ResultObject> DeletePage(int id);
        Task<ResultDataObject<List<Page>>> GetAllPages();
    }
}
