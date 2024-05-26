using expenseManager.Models.Helper;

namespace expenseManager.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Default { get; set; } = false;
    
    public List<RecordCategory> RecordCategories { get; set; } // Navigation property

}