using SdWP.Data.Models;

namespace SdWP.Data.IData
{
    public interface IErrorLogRepository
    {
        Task AddLogAsync(ErrorLog log);
    }
}
