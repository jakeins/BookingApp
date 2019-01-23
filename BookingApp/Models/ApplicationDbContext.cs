using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        {
        }


        public virtual DbSet<Bookings> Bookings { get; set; }
        public virtual DbSet<Resources> Resources { get; set; }
        public virtual DbSet<Rules> Rules { get; set; }
        public virtual DbSet<TreeGroups> TreeGroups { get; set; }
    }
}
