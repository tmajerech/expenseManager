using expenseManager.Data;
using expenseManager.IO;


namespace expenseManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            var outputManager = new ConsoleOutputManager();
            var inputManager = new ConsoleInputManager(outputManager);

            using var db = new DataContext();
            db.Database.EnsureCreated();
            
            var expenseManager = new ExpenseManager(outputManager, inputManager, db);
            expenseManager.Start();
        }
    }
}