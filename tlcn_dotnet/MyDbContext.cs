using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<Account> Account { get; set; }
        public DbSet<ConfirmToken> ConfirmToken { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<BillDetail> BillDetail { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<ChangePasswordToken> ChangePasswordToken { get; set; }
        public DbSet<CartDetail> CartDetail { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Review> Review { get; set; }
    }
}
