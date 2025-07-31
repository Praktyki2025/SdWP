public class DataTableRequest
{
    public int draw { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    public Search search { get; set; } = new Search();
    public List<Order> order { get; set; } = new List<Order>();
    public List<Column> columns { get; set; } = new List<Column>();
}

public class Search
{
    public string? value { get; set; }
    public bool regex { get; set; }
}

public class Order
{
    public int column { get; set; }
    public string? dir { get; set; }  // "asc" or "desc"
}

public class Column
{
    public string? data { get; set; }
    public string? name { get; set; }
    public bool searchable { get; set; }
    public bool orderable { get; set; }
    public Search? search { get; set; }
}
