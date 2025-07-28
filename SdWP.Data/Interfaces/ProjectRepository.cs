using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Interfaces;
using SdWP.Data.Models;

namespace SdWP.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task DeleteProjectAsync(int id)
        {
            var projectToDelete = await _context.Projects.FindAsync(id);
            if (projectToDelete != null)
            {
                _context.Projects.Remove(projectToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.ToListAsync();
        }
    }
}