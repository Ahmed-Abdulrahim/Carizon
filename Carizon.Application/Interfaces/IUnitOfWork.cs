namespace Carizon.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> CommitAsync(CancellationToken cancellationToken);
        void Dispose();
    }
}
