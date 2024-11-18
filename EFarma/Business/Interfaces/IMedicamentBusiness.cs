
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IMedicamentBusiness
    {
        Task<ResultObject> Create(Medicament medicament);
        Task<ResultObject> Delete(int id);
        Task<ResultDataObject<List<Medicament>>> GetAll();
        Task<ResultObject> Update(int id, MedicamentDTO medicamentDto);
    }
}
