using expenseManager.Models;
using expenseManager.Models.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace expenseManager.Interfaces;

public interface IDataContext
{
    DbSet<Record> Records { get; }
    DbSet<Category> Categories { get; }
    DbSet<RecordCategory> RecordCategories { get; }
    DbSet<User> Users { get; }
    
    Task<int> SaveChangesAsync();
    void MarkAsModified(object item);
    
    IDbContextTransaction BeginTransaction(); 
    void CommitTransaction();  
    void RollbackTransaction();
}