using SdWP.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Service.IServices
{
    public interface IProjectInteractionsService
    {
        Task<ProjectUpsertResponseDTO> CreateProjectAsync(ProjectUpsertRequestDTO project);
        Task<ProjectUpsertResponseDTO> EditProjectAsync(ProjectUpsertRequestDTO project);
        Task<ProjectDeleteResponseDTO> DeleteProjectAsync(ProjectDeleteRequestDTO project);
        Task<ProjectUpsertResponseDTO> GetByIdAsync(Guid id);
        Task<List<ProjectUpsertResponseDTO>> GetAllAsync();
    }
}
