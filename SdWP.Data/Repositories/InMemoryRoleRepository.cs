// SdWP.Data.Repositories/InMemoryRoleRepository.cs

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SdWP.Data.Repositories
{
    public class InMemoryRoleRepository : IRoleStore<IdentityRole>
    {
        private readonly List<IdentityRole> _role = new();

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            _role.Add(role);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = _role.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);
            return Task.FromResult(role);
        }


        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            _role.RemoveAll(r => r.Id == role.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var role = _role.FirstOrDefault(r => r.Id == roleId);
            return Task.FromResult(role);
        }

        public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.NormalizedName);

        public Task<string?> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
           => Task.FromResult(role.Id);

        public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            var existingRole = _role.FirstOrDefault(r => r.Id == role.Id);

            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                existingRole.NormalizedName = role.NormalizedName;
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //
        }
    }
}