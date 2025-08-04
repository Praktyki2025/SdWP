using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using System.Linq.Expressions;
using System.Security.Claims;
using SdWP.Data.Interfaces;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

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
                    message: "User is not authenticated",
                    statusCode: StatusCodes.Status401Unauthorized,
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
                    message: "Projects found successfully",
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
                        statusCode: StatusCodes.Status204NoContent);
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
                    message: "Projects found successfully",
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
                List<Project>? projects = await _projectRepository.GetAllAsync();

                if (!user.IsInRole("Admin"))
                {
                    projects = projects.Where(p => p.CreatorUserId == Guid.Parse(userId)).ToList();
                }

                if (user == null || !user.Identity.IsAuthenticated)
                {
                    return ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>.BadResult(
                    message: "You are not authenticated",
                    statusCode: StatusCodes.Status401Unauthorized
                    );
                }

                if (!string.IsNullOrWhiteSpace(request.search?.value))
                {
                    string searchLower = request.search.value.ToLower();
                    projects = projects.Where(p =>
                        (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchLower)) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchLower))
                    ).ToList();
                }

                //sorting
                if (request.order != null && request.order.Count > 0)
                {
                    var order = request.order[0];
                    bool ascending = order.dir == "asc";
                    string? sortColumn = null;
                    if (request.columns != null && request.columns.Count > order.column)
                    {
                        sortColumn = request.columns[order.column].data;
                    }

                    if (!string.IsNullOrEmpty(sortColumn))
                    {
                        projects = ApplyOrdering(projects, sortColumn, ascending);
                    }
                }

                var totalRecords = projects.Count();

                var data = projects
                    .Skip(request.start)
                    .Take(request.length)
                    .Select(project => new ProjectUpsertResponseDTO
                    {
                        Id = project.Id,
                        Title = project.Title,
                        Description = project.Description,
                        CreatedAt = project.CreatedAt,
                        LastModified = project.LastModified,         
                    })
                    .ToList();

                var projectListResponse = new ProjectListResponse<ProjectUpsertResponseDTO>
                {
                    Projects = data,
                    TotalCount = totalRecords,
                    HasMore = request.start + request.length < totalRecords
                };

                return ResultService<ProjectListResponse<ProjectUpsertResponseDTO>>.GoodResult(
                    message: "Projects retrieved successfully",
                    statusCode: StatusCodes.Status200OK,
                    data: projectListResponse
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

        //sorting fn
        private List<Project> ApplyOrdering(List<Project> source, string propertyName, bool ascending)
        {
            if (source == null || string.IsNullOrWhiteSpace(propertyName))
                return source ?? new List<Project>();

            propertyName = FirstCharToUpper(propertyName);

            var propInfo = typeof(Project).GetProperty(propertyName);
            if (propInfo == null)
                return source;

            return ascending
                ? source.OrderBy(x => propInfo.GetValue(x, null)).ToList()
                : source.OrderByDescending(x => propInfo.GetValue(x, null)).ToList();
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
