using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.IData
{
    public interface ICostTypeRepository
    {
        Task<Guid> GetGuidByName(string name);
        Task<List<string>> GetAllNamesAsync();
    }
}
