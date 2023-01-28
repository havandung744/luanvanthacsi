using luanvanthacsi.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace luanvanthacsi.Api.Data
{
    public class luanvanthacsiDbContext : IdentityDbContext
    {
        public luanvanthacsiDbContext(DbContextOptions<luanvanthacsiDbContext> options) : base(options)
        {

        }
        public DbSet<Scientist> scientists { get; set; }
    }
}
