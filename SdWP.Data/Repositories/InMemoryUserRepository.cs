using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;

namespace SdWP.Data.Repositories
{
    public class InMemoryUserRepository :
        IUserStore<User>,
        IUserPasswordStore<User>,
        IUserEmailStore<User>,
        IUserRoleStore<User>
    {
        private readonly List<User> _users = new();
        private readonly Dictionary<Guid, List<string>> _userRoles = new(); 

        public InMemoryUserRepository()
        {
            //
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (_users.Any(u => u.NormalizedEmail == user.NormalizedEmail))
            {
                return Task.FromResult(
                    IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "User with this email already exists."
                    }));
            }

            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }

            user.CreatedAt = DateTime.UtcNow;
            user.LastUpdate = DateTime.UtcNow;
            user.NormalizedUserName = user.UserName?.ToUpper();
            user.NormalizedEmail = user.Email?.ToUpper();
            user.EmailConfirmed = true;

            _users.Add(user);
            _userRoles[user.Id] = new List<string>();

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
            {
                return Task.FromResult(
                    IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "User not found."
                    }));
            }

            var index = _users.IndexOf(existingUser);
            user.LastUpdate = DateTime.UtcNow;
            _users[index] = user;

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _users.RemoveAll(u => u.Id == user.Id);
            _userRoles.Remove(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
                return Task.FromResult<User?>(null);

            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
                return Task.FromResult<User?>(null);

            var user = _users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName.ToUpper());
            return Task.FromResult(user);
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

        public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedEmail))
                return Task.FromResult<User?>(null);

            var user = _users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail.ToUpper());
            return Task.FromResult(user);
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

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException();

            if (!_userRoles.ContainsKey(user.Id))
            {
                _userRoles[user.Id] = new List<string>();
            }

            if (!_userRoles[user.Id].Contains(roleName))
            {
                _userRoles[user.Id].Add(roleName);
            }

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException();

            if (_userRoles.ContainsKey(user.Id))
            {
                _userRoles[user.Id].Remove(roleName);
            }

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (_userRoles.ContainsKey(user.Id))
            {
                return Task.FromResult<IList<string>>(_userRoles[user.Id]);
            }

            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null || string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException();

            if (_userRoles.ContainsKey(user.Id))
            {
                return Task.FromResult(_userRoles[user.Id].Contains(roleName));
            }

            return Task.FromResult(false);
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var usersInRole = _users.Where(u =>
                _userRoles.ContainsKey(u.Id) &&
                _userRoles[u.Id].Contains(roleName)).ToList();

            return Task.FromResult<IList<User>>(usersInRole);
        }
    }
}