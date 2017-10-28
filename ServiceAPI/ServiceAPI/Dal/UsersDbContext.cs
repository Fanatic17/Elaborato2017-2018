using Microsoft.EntityFrameworkCore;

namespace ServiceAPI.Dal
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                //.UseMySql(@"Server=localhost;database=corso;uid=corso;pwd=unict;");
                .UseMySql(@"Server=localhost;database=corso;uid=root;");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Skip shadow types
                if (entityType.ClrType == null)
                    continue;

                entityType.Relational().TableName = entityType.ClrType.Name;
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}