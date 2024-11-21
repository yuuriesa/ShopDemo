using CustomerManagement.Models;

namespace CustomerManagement.Repository
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> GetAll(PaginationFilter validFilter);
        public TEntity GetById(int id);
        public TEntity Add(TEntity entity);
        public TEntity Update(int id, TEntity entity);
        public void Delete(int id);
        public void SaveChanges();
    }
}