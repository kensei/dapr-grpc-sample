using DaprSample.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace DaprSample.Shared.Models
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AuctionGoods> AuctionGoods { get; set; }
    }
}