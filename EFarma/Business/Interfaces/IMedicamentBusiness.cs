
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IMedicamentBusiness
    {
        Task<ResultObject> CreateMedicament(MedicamentDTO medicamentDTO);
        Task<ResultDataObject<IEnumerable<Dictionary<int, string>>>> GetAllMedicaments();
    }
}
