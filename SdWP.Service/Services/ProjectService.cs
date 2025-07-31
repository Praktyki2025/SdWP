using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;
using System.Security.Claims;

namespace SdWP.Service.Services
{
    public class ProjectService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : IProjectService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public async Task<ProjectUpsertResponseDTO> CreateProjectAsync(ProjectUpsertRequestDTO project)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return new ProjectUpsertResponseDTO { Success = false };
            }

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var response = new Project
            {
                Id = Guid.NewGuid(),
                Title = project.Title,
                Description = project.Description,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                CreatorUserId = Guid.Parse(userId)

            };

            await _context.Projects.AddAsync(response);
            await _context.SaveChangesAsync();
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
            var existingProject = await _context.Projects.FindAsync(project.Id);
            if (existingProject == null)
            {
                await Task.CompletedTask;
            }

            existingProject.Title = project.Title;
            existingProject.Description = project.Description;
            existingProject.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ProjectUpsertResponseDTO
            {
                Id = existingProject.Id,
                Title = existingProject.Title,
                Description = existingProject.Description,
                CreatedAt = existingProject.CreatedAt,
                LastModified = existingProject.LastModified
            };
        }

        public async Task<ProjectDeleteResponseDTO> DeleteProjectAsync(Guid projectId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return new ProjectDeleteResponseDTO { Success = false };
            }

            var project = await _context.Projects.FindAsync(projectId);

            if (project == null || !user.IsInRole("Admin") && Guid.Parse(userId) != project.CreatorUserId)
            {
                return new ProjectDeleteResponseDTO { Success = false };
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return new ProjectDeleteResponseDTO { Success = true };
        }

        public async Task<ProjectUpsertResponseDTO> GetByIdAsync(Guid id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);

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

        public IQueryable<ProjectUpsertResponseDTO> GetProjects()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (user.Identity.IsAuthenticated)
            {
                if (user.IsInRole("Admin"))
                {
                    return _context.Projects
                    .Select(p => new ProjectUpsertResponseDTO
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        LastModified = p.LastModified,
                    });
                }
                else
                {
                    return context.Projects
                    .Where(p => p.CreatorUserId == Guid.Parse(userId))
                    .Select(p => new ProjectUpsertResponseDTO
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        LastModified = p.LastModified,
                    });
                }
            }
            return null;
        }
    }
}
