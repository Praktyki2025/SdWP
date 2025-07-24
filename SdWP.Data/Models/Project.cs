using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        
        public Guid CreatorUserId { get; set; }
        public User CreatorUser { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModified { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Valuation> Valuations { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Link> Links { get; set; }

        

        
    }
}
