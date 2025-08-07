using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Requests.Datatable
{
    public class DataTableColumnRequestDTO
    {
        public string? data { get; set; }
        public string? name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public DataTableSearchRequestDTO? search { get; set; }
    }
}
