using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data
{
    public class ApplicationDbContextcs : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContextcs(DbContextOptions<ApplicationDbContextcs> options)
            : base(options)
        {

        }
    }
}
