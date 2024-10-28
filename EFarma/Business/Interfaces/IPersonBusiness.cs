using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPersonBusiness
    {
        Task<ResultDataObject<List<PersonView>>> GetPersonList(string? name, string? cpf);
    }
}