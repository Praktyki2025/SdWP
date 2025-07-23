using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SdWP.Data.Models
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        
        public virtual ICollection<Valuation> Valuations { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
        public virtual ICollection<ErrorLog> ErrorLog { get; set; }
    }
}
