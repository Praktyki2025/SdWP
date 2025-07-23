using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;

namespace SdWP.Data.Repositories
{
    public class InMemoryUserRepository :
        IUserStore<User>,
        IUserPasswordStore<User>,
        IUserEmailStore<User>
    {
        private readonly List<User> _users = new();
        private readonly Dictionary<Guid, string> _passHash = new();

        public InMemoryUserRepository()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.pl",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Name = "Administrator",
                CreatedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow
            };

            var hash = new PasswordHasher<User>();
            user.PasswordHash = hash.HashPassword(user, "admin123");
            _passHash[user.Id] = user.PasswordHash;
            _users.Add(user);
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            user.Id = Guid.NewGuid();
            _users.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index != -1) _users[index] = user;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(userId, out var guid))
            {
                var user = _users.FirstOrDefault(u => u.Id == guid);
                return Task.FromResult(user);
            }

            return Task.FromResult<User?>(null);
        }

        public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = _users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName);
            return Task.FromResult(user);
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);

        public Task<string?> GetUserIdAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id.ToString());

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (passwordHash != null)
            {
                _passHash[user.Id] = passwordHash;
            }
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            _passHash.TryGetValue(user.Id, out var password);
            return Task.FromResult(password);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(_passHash.ContainsKey(user.Id));

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _users.RemoveAll(u => u.Id == user.Id);
            _passHash.Remove(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Email);

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken) => Task.CompletedTask;
        
        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = _users.FirstOrDefault(u => u.Email.ToUpper() == normalizedEmail);
            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email.ToUpper());
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
