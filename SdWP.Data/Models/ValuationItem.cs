namespace SdWP.Data.Models
{
    public class ValuationItem
    {
        public Guid Id { get; set; }
        public Guid ValuationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorUserId { get; set; }
        public User CreatorUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public Guid CostTypeId { get; set; }
        public Guid CostCategoryID { get; set; }
        public Guid UserGroupTypeId { get; set; }
        public decimal Quantity { get; set; }  
        public float UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int RecurrencePeriod { get; set; }
        public string RecurrenceUnit { get; set; }

        public virtual Valuation Valuation { get; set; }
        public virtual CostType CostType { get; set; }
        public virtual CostCategory CostCategory { get; set; }
        public virtual UserGroupType UserGroupType { get; set; }
    }
}
