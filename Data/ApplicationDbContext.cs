using Microsoft.EntityFrameworkCore;
using Web_Api_Timescale_Data.Entities;

namespace Web_Api_Timescale_Data.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ResultEntity> Results { get; set; }
        public DbSet<ValueEntity> Values { get; set; }
    }
}