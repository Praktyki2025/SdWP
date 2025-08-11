using SdWP.Data.Models;
using SdWP.DTO.Responses;
using SdWP.DTO.Requests.Datatable;
using Microsoft.AspNetCore.Identity;

namespace SdWP.Data.IData
{
    public interface IUserRepository
    {
        Task<UserListResponse> FiltredAsync(DataTableRequest request, Guid userId);
        Task<List<UserListResponse>> GetUsersAsync(DataTableRequest request);
        Task<User?> FindByIdAsync(string userId);
        Task<User?> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync (User user);
        Task<IdentityResult> DeleteAsync(User user);
        Task<IdentityResult> UpdateAsync(User user);
        Task<User?> FindByNameAsync(string name);
        Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles);
        Task<SignInResult> PasswordSignInAsync(User user, string password, bool? isPersistent, bool? lockoutOnFailure);
        Task SignOutAsync();
        Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled);
        Task<IdentityResult> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd);
    }
}
