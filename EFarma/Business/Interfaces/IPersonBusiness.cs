using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPersonBusiness
    {
        Task<IEnumerable<PersonView>> GetPersonList(string? name, string? cpf);
    }
}