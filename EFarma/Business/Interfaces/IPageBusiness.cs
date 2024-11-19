using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IPageBusiness
    {
        Task<ResultObject> CreatePage(Page page);
        Task<ResultObject> DeletePage(int id);
        Task<ResultDataObject<List<Page>>> GetAllPages();
    }
}
