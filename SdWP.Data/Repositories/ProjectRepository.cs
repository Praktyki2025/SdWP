using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using System.Linq.Dynamic.Core;

namespace SdWP.Data.Repositories
{
    public class ProjectRepository(ApplicationDbContext context) : IProjectRepository
    {
        private readonly ApplicationDbContext _context = context;
        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects
                .Include(p => p.Users)
                .Include(p => p.Links)
                .Include(p => p.Valuations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProjectListResponse<ProjectUpsertResponseDTO>> FilterAsync(DataTableRequest request, UserRole userRole, Guid userId)
        {
            IQueryable<Project> projects;
            switch (userRole)
            {
                case UserRole.Admin:
                    projects = _context.Projects
                        .AsQueryable();
                    break;
                case UserRole.User:
                    projects = _context.Projects
                        .Where(p => p.CreatorUserId == userId).AsQueryable();
                    break;
                case UserRole.Unknown:
                default:
                    return new ProjectListResponse<ProjectUpsertResponseDTO>()
                    {
                        Projects = [],
                        TotalCount = 0,
                        HasMore = false
                    };
            }

            if (!string.IsNullOrWhiteSpace(request.search?.value))
            {
                string searchLower = request.search.value.ToLower();
                projects = projects.Where(p =>
                    (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchLower))
                );
            }

            var totalRecords = projects.Count();
            Console.WriteLine($"Size - {projects.Count()}");

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

            var data = await projects.AsNoTracking()
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
                .ToListAsync();

            var projectListResponse = new ProjectListResponse<ProjectUpsertResponseDTO>
            {
                Projects = data,
                TotalCount = totalRecords,
                HasMore = request.start + request.length < totalRecords
            };
            return projectListResponse;
        }

        //sorting fn
        private IQueryable<Project> ApplyOrdering(IQueryable<Project> query, string sortColumn, bool ascending)
        {
            var direction = ascending ? "ascending" : "descending";
            return query.OrderBy($"{sortColumn} {direction}");
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        
    }
}
