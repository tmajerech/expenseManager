using expenseManager.Interfaces;
using expenseManager.Models;
using Microsoft.VisualBasic;

namespace expenseManager.IO;

public class ConsoleOutputManager : IOutputManager
{
    public void PrintIntro()
    {
        Console.WriteLine("Welcome to the Expense Manager");
    }

    public void PrintUnsignedMenu()
    {
        Console.WriteLine("====MENU====");
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Log In");
        Console.WriteLine("2. Register");
    }

    public void PrintInternalMenu()
    {
        Console.WriteLine("====MENU====");
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Add record");
        Console.WriteLine("2. List records");
        Console.WriteLine("3. Edit record");
        Console.WriteLine("4. Delete record");
        Console.WriteLine("5. Show balance");
        Console.WriteLine("6. Logout");
        Console.WriteLine("7. Add Category");
        Console.WriteLine("8. Edit Category");
        Console.WriteLine("9. Delete Category");
        Console.WriteLine("10. List Categories");
        Console.WriteLine("11. Filter Records");
        
        Console.WriteLine("12. Import data");
        Console.WriteLine("13. Export data");
        Console.WriteLine("14. Generate statistics");
        Console.WriteLine("==============");
    }

    public void PrintSignupName()
    {
        Console.WriteLine("[Signup] Enter your new name");
    }

    public void PrintSignupPassword()
    {
        Console.WriteLine("[Signup] Enter your new password");
    }

    public void PrintSignupError()
    {
        Console.WriteLine("[Signup] Invalid credentials");
    }

    public void PrintSignupSuccess()
    {
        Console.WriteLine("[Signup] Successful");
    }

    public void PrintLoginName()
    {
        Console.WriteLine("[Login] Enter your name");
    }

    public void PrintLoginPassword()
    {
        Console.WriteLine("[Login] Enter your password");
    }
    
    public void PrintLoginError()
    {
        Console.WriteLine("[Login] Invalid credentials");
    }

    public void PrintLoginSuccess()
    {
        Console.WriteLine("[Login] Successful");
    }

    public void PrintLogout()
    {
        Console.WriteLine("You have been logged out");
    }

    public void PrintNewCategoryName()
    {
        Console.WriteLine("[Create Category] Category name");
    }

    public void PrintNewCategorySuccess()
    {
        Console.WriteLine("[Create Category] Success");
    }
    public void PrintEditCategoryId()
    {
        Console.WriteLine("[Edit Category] Category ID");
    }
    public void PrintEditCategoryName()
    {
        Console.WriteLine("[Edit Category] Category name");
    }

    public void PrintEditCategorySuccess()
    {
        Console.WriteLine("[Edit Category] Success");
    }

    public void PrintDeleteCategoryId()
    {
        Console.WriteLine("[Delete Category] Category ID");
    }

    public void PrintDeleteCategorySuccess()
    {
        Console.WriteLine("[Delete Category] Success");
    }

    public void PrintCategoryNameInvalid()
    {
        Console.WriteLine("Category name is invalid");
    }

    public void PrintCategoryUsedError()
    {
        Console.WriteLine("Category cannot be deleted because is used!");
    }

    public void PrintCurrentBalance(float balance)
    {
        Console.WriteLine($"Your current balance is {balance}");
    }

    public void PrintSelectIDToDelete()
    {
        Console.WriteLine("Select ID to delete");
    }

    public void PrintErrorDeleting()
    {
        Console.WriteLine("Error deleting");
    }

    public void PrintRecordInfo(Record r)
    {
        var categoriesNames = r.RecordCategories?.Select(rc => rc.Category)?.Select(c => c.Name).ToList() ?? []; 
        Console.WriteLine($"{r.Id} {r.Type} {r.Name} {r.Amount} {string.Join(",", categoriesNames)} {r.Date.ToString("yyyy-MM-dd")}");
    }

    public void PrintCategoryInfo(Category category)
    {
        Console.WriteLine($"{category.Id} {category.Name}{(category.Default ? " (Default)":"")}");
    }

    public void PrintAddRecordSelectName()
    {
        Console.WriteLine("[Add record] Select name");
    }

    public void PrintAddRecordSelectCategory()
    {
        Console.WriteLine("[Add record] Select category");
    }

    public void PrintAddRecordSelectDate()
    {
        Console.WriteLine("[Add record] Select date in format YYYY-MM-DD");
    }

    public void PrintAddRecordSelectAmount()
    {
        Console.WriteLine("[Add record] Select amount");
    }

    public void ListCategories(IEnumerable<Category> categories)
    {
        var enumerable = categories.ToList();
        
        if (enumerable.Count > 0)
        {
            Console.WriteLine("Available categories:");
            Console.WriteLine("ID Name");
            foreach (var category in enumerable)
            {
                PrintCategoryInfo(category);
            }
        }
        else
        {
            Console.WriteLine("No categories found");
        }
    }

    public void ListRecords(IEnumerable<Record> records)
    {
        var enumerable = records.ToList();
        
        if (enumerable.Count > 0)
        {
            Console.WriteLine("Available records:");
            Console.WriteLine("ID Type Name Amount Categories Date");
            foreach (var record in enumerable)
            {
                PrintRecordInfo(record);
            }
        }
        else
        {
            Console.WriteLine("No records found");
        }
    }

    public void PrintInputError()
    {
        Console.WriteLine("Invalid input");
    }

    public void PrintInvalidId()
    {
        Console.WriteLine("Selected ID is invalid");
    }

    public void PrintSuccess()
    {
        Console.WriteLine("Success");
    }

    public void PrintSelectId()
    {
        Console.WriteLine("Select ID");
    }

    public void PrintSelectRecordType()
    {
        Console.WriteLine("Select record type");
        Console.WriteLine("0. Income");
        Console.WriteLine("1. Expenditure");
    }

    public void PrintErrorCreating()
    {
        Console.WriteLine("Error creating");
    }

    public void PrintFilterSelectType()
    {
        Console.WriteLine("Filter by type (empty for skip)");
        Console.WriteLine("0. Income");
        Console.WriteLine("1. Expenditure");
    }

    public void PrintFilterSelectCategories()
    {
        Console.WriteLine("Filter by categories (csv, empty for skip)");
    }

    public void PrintFilterSelectDateFrom()
    {
        Console.WriteLine("From (YYYY-MM-DD, empty for skip)");
    }

    public void PrintFilterSelectDateTo()
    {
        Console.WriteLine("To (YYYY-MM-DD, empty for skip)");
    }

    public void PrintImportSelectPath()
    {
        Console.WriteLine("Give us the file path");
    }

    public void PrintImportFileDoesNotExistError()
    {
        Console.WriteLine("FILE DOES NOT EXIST!");
    }

    public void PrintImportAmountImported(int amount)
    {
        Console.WriteLine($"Number of imported rows {amount}");
    }

    public void PrintExportSelectFileName()
    {
        Console.WriteLine("Enter file name");
    }

    public void PrintFileDataInvalid()
    {
        Console.WriteLine("File data invalid!");
    }

    public void PrintFileImportException()
    {
        Console.WriteLine("Exception during importing file");
    }
}