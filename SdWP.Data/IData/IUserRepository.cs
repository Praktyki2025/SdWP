using SdWP.Data.Models;

namespace SdWP.Data.IData
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameAvailableAsync(string username, string password);
    }
}
