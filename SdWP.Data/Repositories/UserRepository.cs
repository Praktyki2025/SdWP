using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace SdWP.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> FindByIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var id)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<UserListResponseDTO>> GetUserAsync(DataTableRequestDTO request)
        {
            IQueryable<User> users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.search.value))
            {
                var searchValue = request.search.value.ToLower();
                users = users.Where(u => u.UserName.ToLower().Contains(searchValue) ||
                                         u.Email.ToLower().Contains(searchValue));
            }

            if (request.order != null && request.order.Count > 0)
            {
                var orderColumn = request.order[0];
                bool ascending = orderColumn.dir == "asc";
                string? sortColumn = null;
                if (request.columns != null && request.columns.Count > orderColumn.column)
                    sortColumn = request.columns[orderColumn.column].data;
                if (!string.IsNullOrEmpty(sortColumn))
                    users = ApplyOrdering(users, sortColumn, ascending);
            }
            else
            {
                users = users.OrderBy(u => u.UserName);
            }

            users = users
                .Skip(request.start)
                .Take(request.length);

            var userList = await users.AsNoTracking()
                .Select(u => new UserListResponseDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.UserName,
                    CreatedAt = u.CreatedAt,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId))
                        .Where(role => role != null)
                        .Select(role => role!.Name)
                        .ToList(),
                    Success = true
                })
                .ToListAsync();

            return userList;

        }

        public async Task<UserListResponseDTO> FiltredAsync(DataTableRequestDTO request, Guid userId)
        {
            IQueryable<User> users;

            users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.search.value))
            {
                var searchValue = request.search.value.ToLower();
                users = users.Where(u => u.UserName.ToLower().Contains(searchValue) ||
                                         u.Email.ToLower().Contains(searchValue));
            }

            var totalRecords = await users.CountAsync();
            if (request.order != null && request.order.Count > 0)
            {
                var orderColumn = request.order[0];
                bool ascending = orderColumn.dir == "asc";
                string? sortColumn = null;
                if (request.columns != null && request.columns.Count > orderColumn.column) sortColumn = request.columns[orderColumn.column].data;

                if (!string.IsNullOrEmpty(sortColumn)) users = ApplyOrdering(users, sortColumn, ascending);
            }

            var data = await users.AsNoTracking()
                .Skip(request.start)
                .Take(request.length)
                .Select(u => new UserListResponseDTO
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId))
                        .Where(role => role != null)
                        .Select(role => role.Name)
                        .ToList()!
                })
                .ToListAsync();

            var firstUser = data.FirstOrDefault();

            var response = new UserListResponseDTO
            {
                Id = firstUser.Id,
                Email = firstUser?.Email ?? string.Empty,
                Name = firstUser?.Name,
                Roles = firstUser?.Roles ?? new List<string>(),
                CreatedAt = firstUser?.CreatedAt,
                Success = true
            };

            return response;
        }

        private IQueryable<User> ApplyOrdering(IQueryable<User> query, string sortColumn, bool ascending)
            => query.OrderBy($"{sortColumn} {(ascending ? "ascending" : "descending")}");


    }
}