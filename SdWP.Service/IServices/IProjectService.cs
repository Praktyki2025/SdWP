using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Service.IServices
{
    public interface IProjectService
    {
        Task<ResultService<ProjectUpsertResponseDTO>> CreateProjectAsync(ProjectUpsertRequestDTO project);
        Task<ResultService<ProjectUpsertResponseDTO>> EditProjectAsync(ProjectUpsertRequestDTO project);
        Task<ResultService<ProjectDeleteResponseDTO>> DeleteProjectAsync(Guid project);
        Task<ResultService<ProjectUpsertResponseDTO>> GetByIdAsync(Guid id);
        Task<ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>> GetProjects(DataTableRequest request);
    }
}
