using SdWP.Data.Models;

namespace SdWP.Frontend.Components.Pages.Project
{
    public class ExampleDb
    {
        public static List<Projects> _projects = new List<Projects>();
        public static List<Projects> GetProjectsAsList()
        {
            return _projects.ToList();
        }

        public static void GenerateList()
        {
            _projects = Enumerable.Range(1, 25).Select(i => new Projects
            {
                Id = Guid.NewGuid(),
                GUID = Guid.NewGuid(),
                CreatorUserId = Guid.NewGuid(),
                Title = $"Project {i}",
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                LastModified = DateTime.UtcNow.AddDays(-i / 2),
                Description = $"Description for project {i}",
                ValuationItems = new List<ValuationItem>(),
                Links = new List<Link>()
            }).ToList();
        }

    }
}
