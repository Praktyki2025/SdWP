using SdWP.Data.Context;
using SdWP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Repositories
{
    public class ErrorLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ErrorLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(ErrorLog log)
        {
            await _context.ErrorLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
