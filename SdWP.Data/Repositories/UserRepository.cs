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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<User?> FindByIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var id)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<UserListResponse>> GetUsersAsync(DataTableRequest request)
        {
            IQueryable<User> users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.search.value))
            {
                var searchValue = request.search.value.ToLower();
                users = users.Where(u => u.UserName.ToLower().Contains(searchValue) ||
                                         u.Email.ToLower().Contains(searchValue));
            }

            var baseQuery = users.Select(u => new
            {
                u.Id,
                u.Email,
                u.UserName,
                u.CreatedAt,
                u.IsActive
            });

            if (request.order != null && request.order.Count > 0)
            {
                var orderColumn = request.order[0];
                bool ascending = orderColumn.dir == "asc";
                string? sortColumn = null;
                if (request.columns != null && request.columns.Count > orderColumn.column)
                    sortColumn = request.columns[orderColumn.column].data;
                
                if (!string.IsNullOrEmpty(sortColumn))
                {
                    if (sortColumn.ToLower() != "roles") baseQuery = ApplyOrderingToBase(baseQuery, sortColumn, ascending);
                }
            }
            else
            {
                baseQuery = baseQuery.OrderBy(u => u.UserName);
            }

            var pagedUsers = baseQuery
                .Skip(request.start)
                .Take(request.length)
                .ToListAsync();

            var userList = new List<UserListResponse>();

            foreach (var user in await pagedUsers)
            {
                var roles = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Join(_context.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => r.Name)
                    .ToListAsync();

                userList.Add(new UserListResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.UserName,
                    CreatedAt = user.CreatedAt,
                    Roles = roles,
                    Success = true,
                    IsActive = user.IsActive
                });

                if (request.order != null && request.order.Count > 0)
                {
                    var orderColumn = request.order[0];
                    bool ascending = orderColumn.dir == "asc";
                    string? sortColumn = null;

                    if (request.columns != null && request.columns.Count > orderColumn.column) sortColumn = request.columns[orderColumn.column].data;

                    if (!string.IsNullOrEmpty(sortColumn) && sortColumn.ToLower() == "roles")
                    {
                        if (ascending)
                        {
                            userList = userList.OrderBy(u => u.Roles.FirstOrDefault() ?? "").ToList();
                        }
                        else
                        {
                            userList = userList.OrderByDescending(u => u.Roles.FirstOrDefault() ?? "").ToList();
                        }
                    }
                }
            }

            return userList;
        }

        public async Task<UserListResponse> FiltredAsync(DataTableRequest request, Guid userId)
        {
            IQueryable<User> users = _context.Users.AsQueryable();

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

            var userData = await users
                .AsNoTracking()
                .Skip(request.start)
                .Take(request.length)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.UserName,
                    u.CreatedAt,
                    u.IsActive
                })
                .FirstOrDefaultAsync();

            if (userData == null) return new UserListResponse { Success = false };

            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userData.Id)
                .Join(
                    _context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name
                )
                .ToListAsync();
            
            var response = new UserListResponse
            {
                Id = userData.Id,
                Email = userData.Email,
                Name = userData.UserName,
                CreatedAt = userData.CreatedAt,
                Roles = roles,
                Success = true,
                IsActive = userData.IsActive
            };

            return response;
        }

        private IQueryable<User> ApplyOrdering(IQueryable<User> query, string sortColumn, bool ascending)
            => query.OrderBy($"{sortColumn} {(ascending ? "ascending" : "descending")}");

        public async Task<User?> FindByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<IdentityResult> CreateAsync(User user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task<bool> RoleExistsAsync(string role)
            => await _roleManager.RoleExistsAsync(role);

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
            => await _userManager.AddToRoleAsync(user, role);

        public async Task<IList<string>> GetRolesAsync(User user)
            => await _userManager.GetRolesAsync(user);

        public async Task<IdentityResult> DeleteAsync(User user)
            => await _userManager.DeleteAsync(user);

        public async Task<IdentityResult> UpdateAsync(User user)
            => await _userManager.UpdateAsync(user);

        public async Task<User?> FindByNameAsync(string name)
            => await _userManager.FindByNameAsync(name);

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
            => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
            => await _userManager.ResetPasswordAsync(user, token, password);

        public async Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles)
        {
            if (roles == null || !roles.Any())
                return IdentityResult.Failed();
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Intersect(roles).ToList();
            if (rolesToRemove.Any())
            {
                return await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }
            return IdentityResult.Success;
        }

        public async Task<SignInResult> PasswordSignInAsync(User user, string password, bool? isPersistent, bool? lockoutOnFailure)
            => await _signInManager.PasswordSignInAsync(user, password, isPersistent ?? false, lockoutOnFailure ?? false);

        public async Task SignOutAsync()
            => await _signInManager.SignOutAsync();

        private IQueryable<T> ApplyOrderingToBase<T>(IQueryable<T> query, string sortColumn, bool ascending)
        {
            var sortExpression = sortColumn.ToLower() switch
            {
                "name" => ascending ? "UserName ascending" : "UserName descending",
                "email" => ascending ? "Email ascending" : "Email descending",
                "createdat" => ascending ? "CreatedAt ascending" : "CreatedAt descending",
                "isactive" => ascending ? "IsActive ascending" : "IsActive descending",
                _ => ascending ? "UserName ascending" : "UserName descending"
            };

            return query.OrderBy(sortExpression);
        }
    }
}