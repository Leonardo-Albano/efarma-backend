using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPersonBusiness
    {
        Task<ResultDataObject<IEnumerable<PersonView>>> GetPersonList(string? name, string? cpf);
    }
}