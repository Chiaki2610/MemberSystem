using MemberSystem.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace MemberSystem.ApplicationCore.Interfaces
{
    public interface ILogRepository
    {
        Task AddLogAsync(Log log);
        Task AddLogDetailAsync(LogDetail logDetail);
    }
}
