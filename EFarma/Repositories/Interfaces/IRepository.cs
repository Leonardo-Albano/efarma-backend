using System.Linq.Expressions;

namespace EFarma.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Get(int id);
        Task<List<TEntity>> GetAll();
        Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, int? take = null);
        Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(List<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);

        void Update(TEntity entity);
    }
}
