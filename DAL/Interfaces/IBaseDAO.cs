namespace DAL.Interfaces
{
    public interface IBaseDAO<TEntity>
    {
        TEntity? SelectById(Guid id);
        IEnumerable<TEntity> SelectAll();
        TEntity Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(Guid id);
    }
}
