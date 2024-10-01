
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IMedicamentBusiness
    {
        Task<DataResult<IEnumerable<Dictionary<int, string>>>> GetAllMedicaments();
    }
}
