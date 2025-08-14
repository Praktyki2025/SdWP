using Microsoft.AspNetCore.Http;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using System.Security.Claims;
using SdWP.Data.IData;
using SdWP.DTO.Requests.Datatable;

namespace SdWP.Service.Services
{
    public class ProjectService(IProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor) : IProjectService
    {
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public async Task<ResultService<ProjectUpsertResponseDTO>> CreateProjectAsync(ProjectUpsertRequestDTO project)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    return ResultService<ProjectUpsertResponseDTO>.BadResult(
                        message: "User is not authenticated",
                        statusCode: StatusCodes.Status401Unauthorized
                        );
                }

                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var response = new Project
                {
                    Id = Guid.NewGuid(),
                    Title = project.Title,
                    Description = project.Description,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    CreatorUserId = Guid.Parse(userId)

                };

                await _projectRepository.AddAsync(response);

                var result = new ProjectUpsertResponseDTO
                {
                    Id = response.Id,
                    Title = response.Title,
                    Description = response.Description,
                    CreatedAt = response.CreatedAt,
                    LastModified = response.LastModified
                };

                return ResultService<ProjectUpsertResponseDTO>.GoodResult(
                    message: "Project created",
                    statusCode: StatusCodes.Status201Created,
                    data: result
                    );
            }
            catch (Exception ex)
            {
                return ResultService<ProjectUpsertResponseDTO>.BadResult(
                    $"An error occurred during creating project: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
        
        public async Task<ResultService<ProjectUpsertResponseDTO>> EditProjectAsync(ProjectUpsertRequestDTO project)
        {
            try
            {
                if (project == null || project.Id == null)
                {
                    return ResultService<ProjectUpsertResponseDTO>.BadResult(
                        message: "Project data is invalid.",
                        statusCode: StatusCodes.Status400BadRequest
                        );
                }

                var existingProject = await _projectRepository.GetByIdAsync((Guid)project.Id);
                if (existingProject == null)
                {
                    return ResultService<ProjectUpsertResponseDTO>.BadResult(
                        message: "Project not found.",
                        statusCode: StatusCodes.Status204NoContent
                        );
                }

                var user = _httpContextAccessor.HttpContext?.User;
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return ResultService<ProjectUpsertResponseDTO>.BadResult(
                        message: "User is not authentificated",
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                var fetchedProject = await _projectRepository.GetByIdAsync((Guid)project.Id);

                if (project == null || !user.IsInRole("Admin") && Guid.Parse(userId) != fetchedProject.CreatorUserId)
                {
                    return ResultService<ProjectUpsertResponseDTO>.BadResult(
                        message: "You don't have permissions to edit this project.",
                        statusCode: StatusCodes.Status403Forbidden);
                }

                existingProject.Title = project.Title;
                existingProject.Description = project.Description;
                existingProject.LastModified = DateTime.UtcNow;

                await _projectRepository.UpdateAsync(existingProject);

                var result = new ProjectUpsertResponseDTO
                {
                    Id = existingProject.Id,
                    Title = existingProject.Title,
                    Description = existingProject.Description,
                    CreatedAt = existingProject.CreatedAt,
                    LastModified = existingProject.LastModified
                };

                return ResultService<ProjectUpsertResponseDTO>.GoodResult(
                    message: "Project edited successfully",
                    statusCode: StatusCodes.Status200OK,
                    data: result
                    );
            }
            catch (Exception ex)
            {
                return ResultService<ProjectUpsertResponseDTO>.BadResult(
                    $"An error occurred during editing project: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }

        }

        public async Task<ResultService<ProjectDeleteResponseDTO>> DeleteProjectAsync(Guid projectId)
        {
            try {                
                var user = _httpContextAccessor.HttpContext?.User;
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return ResultService<ProjectDeleteResponseDTO>.BadResult(
                        message: "User is not authentificated",
                        statusCode: StatusCodes.Status401Unauthorized);
                }

                var project = await _projectRepository.GetByIdAsync(projectId);

                if (project == null || !user.IsInRole("Admin") && Guid.Parse(userId) != project.CreatorUserId)
                {
                    return ResultService<ProjectDeleteResponseDTO>.BadResult(
                        message: "You don't have permissions to delete this project.",
                        statusCode: StatusCodes.Status403Forbidden);
                }

                await _projectRepository.DeleteAsync(projectId);

                return ResultService<ProjectDeleteResponseDTO>.GoodResult(
                    message: "Project was deleted.",
                    statusCode: StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ResultService<ProjectDeleteResponseDTO>.BadResult(
                    $"An error occurred during deleting project: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<ResultService<ProjectUpsertResponseDTO>> GetByIdAsync(Guid id)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);

                if (project == null)
                {
                    return ResultService<ProjectUpsertResponseDTO>.BadResult(
                        message: "Project not found",
                        statusCode: StatusCodes.Status404NotFound);
                }

                var result = new ProjectUpsertResponseDTO
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    LastModified = project.LastModified
                };

                return ResultService<ProjectUpsertResponseDTO>.GoodResult(
                    message: "Project fetched successfully",
                    statusCode: StatusCodes.Status200OK,
                    data: result
                    );
            }
            catch (Exception ex)
            {
                return ResultService<ProjectUpsertResponseDTO>.BadResult(
                    $"An error occurred during fetching project project: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }

        }

        public async Task<ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>> GetProjects(DataTableRequest request)
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (user == null || !user.Identity.IsAuthenticated)
                {
                    return ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>.BadResult(
                    message: "You are not authorized.",
                    statusCode: StatusCodes.Status401Unauthorized
                    );
                }

                ProjectListResponse<ProjectUpsertResponseDTO> projects;
                UserRole role = UserRole.Unknown;
                if (user.IsInRole("Admin"))
                {
                    role = UserRole.Admin;
                }
                else if (user.IsInRole("User"))
                {
                    role = UserRole.User;
                }

                if(role == UserRole.Unknown)
                {
                    return ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>.BadResult(
                        message: "You don't have permissions to view projects.",
                        statusCode: StatusCodes.Status403Forbidden
                    );
                }

                projects = await _projectRepository.FilterAsync(request, role, Guid.Parse(userId));

                return ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>.GoodResult(
                    message: "Projects retrieved successfully",
                    statusCode: StatusCodes.Status200OK,
                    data: projects
                    );
            }
            catch (Exception ex)
            {
                return ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>.BadResult(
                    $"An error occurred during fetching projects: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
