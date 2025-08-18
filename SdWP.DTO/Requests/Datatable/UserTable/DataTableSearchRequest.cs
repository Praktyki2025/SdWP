using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Requests.Datatable.UserTable
{
    public class DataTableSearchRequest
    {
        public string? value { get; set; }
        public bool regex { get; set; }
    }
}
