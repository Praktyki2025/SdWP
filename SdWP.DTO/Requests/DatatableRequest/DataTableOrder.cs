
namespace SdWP.DTO.Requests.Datatable
{
    public class DataTableOrder
    {
        public int column { get; set; }
        public string? dir { get; set; }  // "asc" or "desc"
    }
}
