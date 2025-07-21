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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PrefferedLanguage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        
    }
}
