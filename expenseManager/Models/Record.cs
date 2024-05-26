using expenseManager.Enums;
using expenseManager.Models.Helper;

namespace expenseManager.Models;

public class Record
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RecordType Type { get; set; }
    public DateTime Date { get; set; }
    public float Amount { get; set; }
    public int UserId { get; set; } // Foreign key
    public User User { get; set; } // Navigation property
    public List<RecordCategory> RecordCategories { get; set; } // Navigation property

    
}