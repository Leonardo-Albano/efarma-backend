
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IMedicamentBusiness
    {
        Task<ResultDataObject<IEnumerable<Dictionary<int, string>>>> GetAllMedicaments();
    }
}
