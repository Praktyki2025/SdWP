using SdWP.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdWP.Data.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(int id);
        Task<Project?> GetProjectByIdAsync(int id);
        Task<List<Project>> GetAllProjectsAsync();
    }
}