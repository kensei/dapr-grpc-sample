using DaprSample.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DaprSample.Shared.Models
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AuctionGoods> AuctionGoods { get; set; }
    }
}