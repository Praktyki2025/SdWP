using SdWP.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdWP.Data.IData
{
    public interface IProjectRepository
    {
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task DeleteProjectAsync(Guid id);
        Task<Project?> GetProjectByIdAsync(Guid id);
        Task<List<Project>> GetAllProjectsAsync();
        Task<List<Project>> GetProjectsByCreatorUserIdAsync(Guid userId);
    }
}