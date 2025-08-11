namespace SdWP.Data.Models
{
    public class CostType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ValuationItem> ValuationItems {get; set;}       
    }
}
