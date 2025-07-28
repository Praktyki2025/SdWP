using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Models
{
    public class CostCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ValuationItem> ValuationItems { get; set; }
    }
}
