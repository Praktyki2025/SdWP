using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Repositories
{
    public class UserGroupTypeRepository : IUserGroupTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public UserGroupTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> GetGuidByName(string name)
        {
            var userGroupType = await _context.UserGroupTypes
                .Where(ugt => ugt.Name == name)
                .Select(ugt => ugt.Id)
                .FirstOrDefaultAsync();

            if (userGroupType == Guid.Empty) throw new KeyNotFoundException($"User group type with name '{name}' not found.");

            return userGroupType;
        }


        public Task<List<string>> GetAllNamesAsync()
        => _context.UserGroupTypes.Select(ugt => ugt.Name).ToListAsync();
    }
}
