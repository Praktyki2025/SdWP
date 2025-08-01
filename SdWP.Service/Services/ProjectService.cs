using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;
using System.Linq.Expressions;
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

        public async Task<(List<ProjectUpsertResponseDTO> projects, int totalRecords)> GetProjects(DataTableRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var projects = _context.Projects
                    .Select(p => new ProjectUpsertResponseDTO
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,        
                        CreatedAt = p.CreatedAt,
                        LastModified = p.LastModified,
                    });

            if (user.Identity.IsAuthenticated)
            {
                if (user.IsInRole("Admin"))
                {
                    projects = _context.Projects
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
                    projects = context.Projects
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
                if (!string.IsNullOrWhiteSpace(request.search?.value))
                {
                    var searchLower = request.search.value.ToLower();
                    projects = projects.Where(p =>
                        (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchLower)) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchLower)));
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
                    .ToList();

                foreach(var proj in projects)
                {
                    Console.WriteLine($"{proj.Title}");
                }
                return (data, totalRecords);
            }
            return (new List<ProjectUpsertResponseDTO>(), 0);
        }

        //sorting fn
        private IQueryable<ProjectUpsertResponseDTO> ApplyOrdering(IQueryable<ProjectUpsertResponseDTO> source, string propertyName, bool ascending)
        {
            if (string.IsNullOrEmpty(propertyName))
                return source;

            //normalize prop
            propertyName = FirstCharToUpper(propertyName);

            var param = Expression.Parameter(typeof(ProjectUpsertResponseDTO), "p");
            var property = Expression.PropertyOrField(param, propertyName);
            var sortLambda = Expression.Lambda(property, param);

            string methodName = ascending ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(ProjectUpsertResponseDTO), property.Type)
                .Invoke(null, new object[] { source, sortLambda });

            return (IQueryable<ProjectUpsertResponseDTO>)result;
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
