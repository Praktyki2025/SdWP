namespace SdWP.DTO.Requests.Datatable
{
    public class DataTableRequest
    {
        public int start { get; set; }
        public int length { get; set; }
        public DataTableSearch search { get; set; } = new DataTableSearch();
        public List<DataTableOrder> order { get; set; } = new List<DataTableOrder>();
        public List<DataTableColumn> columns { get; set; } = new List<DataTableColumn>();
    }
}
