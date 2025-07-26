using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using SdWP.Data.Repositories;
using SdWP.Data.Models;

namespace SdWP.Service.Services
{
    public class ProjectInteractionsService(InMemoryProjectRepository repo) : IProjectInteractionsService
    {
        private readonly InMemoryProjectRepository _projectRepository = repo;
        public async Task<ProjectUpsertResponseDTO> CreateProjectAsync(ProjectUpsertRequestDTO project)
        {
            var response = new Project
            {
                Id = Guid.NewGuid(), // Simulating a new project ID
                Title = project.Title,
                Description = project.Description,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(response);
            Console.WriteLine($"{_projectRepository.GetSize()}");

            return new ProjectUpsertResponseDTO
            {
                Id = response.Id,
                Title = response.Title,
                Description = response.Description,
                CreatedAt = response.CreatedAt,
                LastModified = response.LastModified
            };
        }
        public async Task<ProjectUpsertResponseDTO> EditProjectAsync(ProjectUpsertRequestDTO project)
        {

            var response = new Project
            {
                Id = (Guid)project.Id, // Simulating a new project ID
                Title = project.Title,
                Description = project.Description,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };


            await _projectRepository.UpdateAsync(response);

            return new ProjectUpsertResponseDTO
            {
                Id = response.Id,
                Title = response.Title,
                Description = response.Description,
                CreatedAt = response.CreatedAt,
                LastModified = response.LastModified
            };
        }
        public Task<ProjectDeleteResponseDTO> DeleteProjectAsync(Guid project)
        {
            _projectRepository.DeleteAsync(project);
            Console.WriteLine($"{_projectRepository.GetSize()}");
            return Task.FromResult(new ProjectDeleteResponseDTO() { Success = true });
        }

        public async Task<ProjectUpsertResponseDTO> GetByIdAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return null;
            return new ProjectUpsertResponseDTO
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                LastModified = project.LastModified
            };
        }

        public async Task<List<ProjectUpsertResponseDTO>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(p => new ProjectUpsertResponseDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                LastModified = p.LastModified
            }).ToList();

        }
    }
}
