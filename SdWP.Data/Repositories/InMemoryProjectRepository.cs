using SdWP.Data.Models;

public class InMemoryProjectRepository
{
    private readonly List<Project> _projects = new();

    public int GetSize()
    {
        return _projects.Count();
    }
    public Task AddAsync(Project project)
    {
        _projects.Add(project);
        return Task.CompletedTask;
    }

    public Task<Project> GetByIdAsync(Guid id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(project);
    }

    public Task UpdateAsync(Project project)
    {
        var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
        if (existing != null)
        {
            existing.Title = project.Title;
            existing.Description = project.Description;
            existing.LastModified = project.LastModified;
            existing.Users = project.Users;
            existing.Links = project.Links;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        if (project != null)
        {
            _projects.Remove(project);
        }
        return Task.CompletedTask;
    }

    public Task<List<Project>> GetAllAsync()
    {
        return Task.FromResult(_projects.ToList());
    }
}
