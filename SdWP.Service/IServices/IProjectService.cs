using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Requests.ProjectRequests;
using SdWP.DTO.Responses.ProjectRequests;
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
        Task<ResultService<ProjectResponse>> CreateProjectAsync(ProjectCreateRequest project);
        Task<ResultService<ProjectResponse>> EditProjectAsync(ProjectEditRequest project);
        Task<ResultService<ProjectDeleteResponse>> DeleteProjectAsync(ProjectDeleteRequest project); 
        Task<ResultService<ProjectResponse>> GetProjectAsync(Guid id);
        Task<ResultService<ProjectListResponse<ProjectResponse>>> GetProjects(DataTableRequest request);
    }
}
