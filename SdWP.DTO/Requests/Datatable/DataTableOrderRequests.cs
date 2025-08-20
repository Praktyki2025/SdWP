using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Requests.Datatable
{
    public class DataTableOrderRequests
    {
        public int column { get; set; }
        public string? dir { get; set; } // "asc" or "desc"
    }
}
