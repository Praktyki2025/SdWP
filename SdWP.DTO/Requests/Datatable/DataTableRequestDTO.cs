namespace SdWP.DTO.Requests.Datatable
{
    public class DataTableRequestDTO
    {
        public int Draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public DataTableSearchRequestDTO search { get; set; } = new DataTableSearchRequestDTO();
        public List<DataTableOrderRequestsDTO> order { get; set; } = new List<DataTableOrderRequestsDTO>();
        public List<DataTableColumnRequestDTO> columns { get; set; } = new List<DataTableColumnRequestDTO>();
    }
}
