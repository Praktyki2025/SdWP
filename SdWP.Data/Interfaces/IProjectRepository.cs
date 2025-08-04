using SdWP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<Project?> GetByIdAsync(Guid id);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Guid id);
        Task<List<Project>> GetAllAsync();
    }
}
