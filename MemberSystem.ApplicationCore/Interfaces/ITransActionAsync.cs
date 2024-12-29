using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces
{
    public interface ITransActionAsync
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
