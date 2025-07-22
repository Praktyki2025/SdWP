using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Models
{
    public class Valuation
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public virtual User CreatorUser { get; set; }
        public virtual Projects Projects { get; set; } // Rename from Projects to Project
        public virtual ICollection<ValuationItem> ValuationItems { get; set; }
        public virtual ICollection<Link> Links { get; set; }
    }
}
