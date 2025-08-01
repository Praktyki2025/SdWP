using SdWP.Data.Models;

namespace SdWP.Data.IData
{
    internal interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameAvailableAsync(string username, string password);
    }
}
