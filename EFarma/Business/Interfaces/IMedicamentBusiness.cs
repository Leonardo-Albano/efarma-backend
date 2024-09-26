
namespace EFarma.Business.Interfaces
{
    public interface IMedicamentBusiness
    {
        Task<IEnumerable<Dictionary<int, string>>> GetAllMedicaments();
    }
}
