using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Responses.Valuation;

namespace SdWP.Data.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly ApplicationDbContext _context;

        public LinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Link> AddLinkAsync(LinkResponse response)
        {
            var link = new Link
            {
                Id = Guid.NewGuid(),
                Name = response.Name,
                LinkUrl = response.LinkUrl,
                Description = response.Description ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                ValuationId = response.ValuationId,
                ProjectId = response.ProjectId
            };

            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            return link;
        }

        public async Task<List<Link>> GetAllLinksToProject(Guid id)
            => await _context.Links
                .Include(l => l.Valuation)
                .Where(l => l.ProjectId == id)
                .ToListAsync();


        public async Task<bool> DeleteLinkAsync(Guid id)
        {
            var link = await _context.Links.FindAsync(id);
            if (link == null) return false;
            _context.Links.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FindLinkByIdAsync(Guid id)
            => await _context.Links.AnyAsync(l => l.Id == id);

        public async Task<Link> UpdateLinkAsync(UpdateLinkResponse response)
        {
            var link = await _context.Links.FirstOrDefaultAsync(v => v.Id == response.Id);
            if (link == null) throw new Exception("Link not found");

            link.Name = response.Name ?? link.Name;
            link.Description = response.Description ?? link.Description;
            link.LinkUrl = response.LinkUrl ?? link.LinkUrl;
            link.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return link;
        }
    }
}

