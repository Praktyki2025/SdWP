using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Interfaces;
using SdWP.Data.Models;
using SdWP.DTO.Requests;

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

    public async Task<List<Project>> GetAllAsync(bool isAdminRequest)
    {
        var projects = new List<Project>();
        if (isAdminRequest)
        {
            projects = _context.Projects
            .Select(p => new Project
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                LastModified = p.LastModified,
            }).ToList();
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
}