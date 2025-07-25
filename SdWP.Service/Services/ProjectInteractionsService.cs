using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class ProjectInteractionsService : IProjectInteractionsService
    {
        public Task<ProjectUpsertResponseDTO> CreateProjectAsync(ProjectUpsertRequestDTO project)
        {
            return Task.FromResult(new ProjectUpsertResponseDTO
            {
                Id = Guid.NewGuid(),
                Title = project.Title,
                Description = project.Description
            });
        }
        public Task<ProjectUpsertResponseDTO> EditProjectAsync(ProjectUpsertRequestDTO project)
        {
            // Implement the logic to edit a project
            // For now, returning a dummy response
            return Task.FromResult(new ProjectUpsertResponseDTO
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description
            });
        }
        public Task<ProjectDeleteResponseDTO> DeleteProjectAsync(ProjectDeleteRequestDTO project)
        {
            // Implement the logic to delete a project
            // For now, returning a dummy response
            return Task.FromResult(new ProjectDeleteResponseDTO());
        }
    }
}
