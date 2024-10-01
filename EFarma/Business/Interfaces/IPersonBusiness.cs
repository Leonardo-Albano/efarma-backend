using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPersonBusiness
    {
        Task<DataResult<IEnumerable<PersonView>>> GetPersonList(string? name, string? cpf);
    }
}