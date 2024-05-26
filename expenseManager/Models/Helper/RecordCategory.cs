namespace expenseManager.Models.Helper;

public class RecordCategory
{
    public int RecordId { get; set; }
    public Record Record { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}