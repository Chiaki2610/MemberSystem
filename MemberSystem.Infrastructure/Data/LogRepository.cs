using MemberSystem.ApplicationCore.Entities;
using MemberSystem.ApplicationCore.Interfaces;

namespace MemberSystem.Infrastructure.Data
{
    public class LogRepository : ILogRepository
    {
        private readonly MemberSystemContext _context;

        public LogRepository(MemberSystemContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
