using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class PrescriptionItemRepository : Repository<PrescriptionItem>, IPrescriptionItemRepository
    {
        public PrescriptionItemRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<PrescriptionItem>> GetPrescriptionItems(int prescriptionId)
        {
            return await DataContext.PrescriptionItems
                .Include(p => p.Medicament)
                .Where(p => p.PrescriptionId == prescriptionId)
                .ToListAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}
