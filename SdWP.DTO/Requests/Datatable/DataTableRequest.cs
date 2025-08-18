using SdWP.DTO.Requests.Datatable.UserTable;

namespace SdWP.DTO.Requests.Datatable
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public DataTableSearchRequest search { get; set; } = new DataTableSearchRequest();
        public List<DataTableOrderRequests> order { get; set; } = new List<DataTableOrderRequests>();
        public List<DataTableColumnRequest> columns { get; set; } = new List<DataTableColumnRequest>();
    }
}
