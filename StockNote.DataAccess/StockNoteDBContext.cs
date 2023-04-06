using Microsoft.EntityFrameworkCore;
using StockNote.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace StockNote.DataAccess
{
    public class StockNoteDBContext:DbContext
    {
        public DbSet<Unit> Units { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<StockInWarehouse> StockInWarehouses { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public StockNoteDBContext(DbContextOptions<StockNoteDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // add hidden attribute to database table
            // this fields will not be shown in objects
            var allEntities = builder.Model.GetEntityTypes();
            foreach (var entity in allEntities)
            {
                entity.AddProperty("CreatedDate", typeof(DateTime));
                entity.AddProperty("UpdatedDate", typeof(DateTime));
                entity.AddProperty("RowVersion", typeof(Byte[]));
            }
            base.OnModelCreating(builder);
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e =>
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                entityEntry.Property("UpdatedDate").CurrentValue = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property("CreatedDate").CurrentValue = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}