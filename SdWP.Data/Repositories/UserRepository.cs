using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using System.Linq.Dynamic.Core;

namespace SdWP.Data.Repositories
{
    public class UserRepository :
        IUserStore<User>,
        IUserPasswordStore<User>,
        IUserEmailStore<User>,
        IUserRoleStore<User>
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (user.Id == Guid.Empty) user.Id = Guid.NewGuid();

            var exists = await _context.Users
                .AnyAsync(
                    u => u.Id == user.Id || 
                    u.NormalizedUserName == user.UserName.ToUpper(), 
                    cancellationToken
                );

            if (exists)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });
            }

            user.CreatedAt = DateTime.UtcNow;
            user.LastUpdate = DateTime.UtcNow;
            user.NormalizedUserName = user.UserName?.ToUpper();
            user.NormalizedEmail = user.Email?.ToUpper();
            user.EmailConfirmed = true;

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var exists = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            if (exists == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });
            }

            user.LastUpdate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(userId, out var id)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName.ToUpper(), cancellationToken);
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string?> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult<string?>(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public void Dispose()
        {
            // 
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.Email);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedEmail)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail.ToUpper(), cancellationToken);
        }

        public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Task.FromResult(user.EmailConfirmed);
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(
                r => r.Name == roleName,
                cancellationToken
                );

            if (role == null) throw new InvalidOperationException($"Role {roleName}does not exist.");

            _context.UserRoles.Add(new IdentityUserRole<Guid>
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(
                r => r.Name == roleName,
                cancellationToken
            );

            if (role == null) throw new InvalidOperationException($"Role {roleName} does not exist.");

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var roleName = _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId))
                .Where(role => role != null)
                .Select(role => role.Name)
                .ToList();

            return Task.FromResult<IList<string>>(roleName);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(user));

           var isInRole = _context.UserRoles
                .Any(ur => ur.UserId == user.Id && 
                           _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == roleName));
            return Task.FromResult(isInRole);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);

            if (role == null)
            {
                return new List<User>();
            }

            var usersInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId)
                .ToListAsync(cancellationToken);

            var users = await _context.Users
                .Where(u => usersInRole.Contains(u.Id))
                .ToListAsync(cancellationToken);

            return users;
        }

        public async Task<List<(User user, List<string> Roles)>> GetUserRoleAsync(DataTableRequestDTO request ,CancellationToken cancellationToken)
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
                .Select(u => new
                {
                    User = u,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId))
                        .Where(role => role != null)
                        .Select(role => role.Name)
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return userList
                .Select(u => (u.User, u.Roles))
                .ToList();

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