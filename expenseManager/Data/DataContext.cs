
using expenseManager.Models;
using System.Collections.Generic;
using expenseManager.Interfaces;
using expenseManager.Models.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace expenseManager.Data;

public class DataContext : DbContext, IDataContext
{
    // public DataContext(DbContextOptions<DataContext> options) : base(options)
    // {
    //     
    // }
    public DbSet<Record> Records { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RecordCategory> RecordCategories { get; set; }
    public DbSet<User> Users { get; set; }
    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

    public void MarkAsModified(object item)
    {
        Entry(item).State = EntityState.Modified;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=expenseManager.db");

    public IDbContextTransaction BeginTransaction()
    {
        return Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        Database.CommitTransaction();
    }

    public void RollbackTransaction()
    {
        Database.RollbackTransaction();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuring the many-to-many relationship
        modelBuilder.Entity<RecordCategory>()
            .HasKey(rc => new { rc.RecordId, rc.CategoryId }); // Composite key

        modelBuilder.Entity<RecordCategory>()
            .HasOne(rc => rc.Record)
            .WithMany(r => r.RecordCategories)
            .HasForeignKey(rc => rc.RecordId);

        modelBuilder.Entity<RecordCategory>()
            .HasOne(rc => rc.Category)
            .WithMany(c => c.RecordCategories)
            .HasForeignKey(rc => rc.CategoryId);
        
        // Ensuring Name is unique
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique(); 
        
        // one:many relationship author-records
        modelBuilder.Entity<Record>()
            .HasOne(r => r.User)
            .WithMany(u => u.Records)
            .HasForeignKey(b => b.UserId);
    }
}