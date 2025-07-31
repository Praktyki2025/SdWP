using SdWP.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Service.IServices
{
    public interface IProjectService
    {
        Task<ProjectUpsertResponseDTO> CreateProjectAsync(ProjectUpsertRequestDTO project);
        Task<ProjectUpsertResponseDTO> EditProjectAsync(ProjectUpsertRequestDTO project);
        Task<ProjectDeleteResponseDTO> DeleteProjectAsync(Guid project);
        Task<ProjectUpsertResponseDTO> GetByIdAsync(Guid id);
        IQueryable<ProjectUpsertResponseDTO> GetProjects();
    }
}
