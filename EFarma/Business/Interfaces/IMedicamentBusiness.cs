
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IMedicamentBusiness
    {
        Task<ResultObject> CreateMedicament(MedicamentDTO medicamentDTO);
        Task<ResultObject> DeleteMedicament(int id);
        Task<ResultDataObject<List<Dictionary<int, string>>>> GetAllMedicaments();
    }
}
