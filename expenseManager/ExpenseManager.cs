using System.Globalization;
using System.Text;
using expenseManager.Enums;
using expenseManager.Exceptions;
using expenseManager.Helpers;
using expenseManager.Interfaces;
using expenseManager.IO;
using expenseManager.Models;
using expenseManager.Models.Helper;
using Microsoft.EntityFrameworkCore;

namespace expenseManager;

public class ExpenseManager
{
    private readonly IOutputManager _outputManager;
    private readonly IInputManager _inputManager;
    private readonly IDataContext _db;
    private User? _user;

    private readonly List<string> _defaultCategories = ["Food", "Pets", "Car", "Rent", "Wage"]; 

    public ExpenseManager(IOutputManager outputManager, IInputManager inputManager, IDataContext db)
    {
        _outputManager = outputManager;
        _inputManager = inputManager;
        _db = db;
        
        CreateDefaultCategoriesIfNeeded();
    }

    public void Start()
    {
        _outputManager.PrintIntro();
        
        //main select loop
        while (true)
        {
            if (_user is null)
            {
                // unsigned menu
                _outputManager.PrintUnsignedMenu();
                var menuSelection = _inputManager.ParseUnsignedMenuSelection();
                switch (menuSelection)
                {
                    case UnsignedMenuOptions.LogIn:
                        HandleLogin();
                        break;
                    case UnsignedMenuOptions.Register:
                        HandleRegister();
                        break;
                }
            }
            else
            {
                //signed in menu
                _outputManager.PrintInternalMenu();
                var menuSelection = _inputManager.ParseInternalMenuSelection();
                switch (menuSelection)
                {
                    case InternalMenuOptions.AddRecord:
                        HandleAddRecord();
                        break;
                    case InternalMenuOptions.EditRecord:
                        HandleEditRecord();
                        break;
                    case InternalMenuOptions.DeleteRecord:
                        HandleDeleteRecord();
                        break;
                    case InternalMenuOptions.ListRecords:
                        HandleListRecords();
                        break;
                    case InternalMenuOptions.ShowBalance:
                        HandleShowBalance();
                        break;
                    case InternalMenuOptions.Logout:
                        HandleLogout();
                        break;
                    case InternalMenuOptions.AddCategory:
                        HandleAddCategory();
                        break;
                    case InternalMenuOptions.EditCategory:
                        HandleEditCategory();
                        break;
                    case InternalMenuOptions.DeleteCategory:
                        HandleDeleteCategory();
                        break;
                    case InternalMenuOptions.ListCategories:
                        HandleListCategories();
                        break;
                    case InternalMenuOptions.FilterRecords:
                        HandleFilterRecords();
                        break;
                    case InternalMenuOptions.ImportData:
                        HandleImportData();
                        break;
                    case InternalMenuOptions.ExportData:
                        HandleExportData();
                        break;
                    case InternalMenuOptions.GenerateStatistics:
                        HandleGenerateStatistics();
                        break;
                }
            }

        }
    }

    private void HandleListCategories()
    {
        _outputManager.ListCategories(_db.Categories);
    }

    private void HandleAddCategory()
    {
        _outputManager.PrintNewCategoryName();
        var newName = _inputManager.ParseString();
        
        //check name is unique
        if (_db.Categories.Any(c => c.Name == newName))
        {
            _outputManager.PrintCategoryNameInvalid();
            return;
        }
        
        var newCategory = new Category
        {
            Name = newName
        };
        _db.Categories.Add(newCategory);
        _db.SaveChangesAsync();
        _outputManager.PrintNewCategorySuccess();
    }

    private void HandleEditCategory()
    {
        var availableCatIds = _db.Categories.Where(c => !c.Default).Select(c => c.Id).ToList();
        
        _outputManager.PrintEditCategoryId();
        var catId = _inputManager.ParseId(availableCatIds);
        
        //get the category
        var cat = _db.Categories.Find(catId);
        if (cat is null)
        {
            _outputManager.PrintInputError();
            return;
        }
        
        _outputManager.PrintEditCategoryName();
        var newName = _inputManager.ParseString();
        
        //check name is unique
        if (_db.Categories.Any(c => c.Name == newName))
        {
            _outputManager.PrintCategoryNameInvalid();
            return;
        }

        cat.Name = newName;

        _db.SaveChangesAsync();
        
        _outputManager.PrintEditCategorySuccess();
    }
    
    private void HandleDeleteCategory()
    {
        var availableCatIds = _db.Categories.Where(c => !c.Default).Select(c => c.Id).ToList();
        
        _outputManager.PrintDeleteCategoryId();
        var catId = _inputManager.ParseId(availableCatIds);
        
        //get the category
        var cat = _db.Categories.Find(catId);
        if (cat is null)
        {
            _outputManager.PrintInputError();
            return;
        }

        if (_db.RecordCategories.Any(rc => rc.CategoryId == catId))
        {
            _outputManager.PrintCategoryUsedError();
            return;
        }

        _db.Categories.Remove(cat);
        _db.SaveChangesAsync();
        _outputManager.PrintDeleteCategorySuccess();
    }
    
    private void HandleLogout()
    {
        _user = null;
        _outputManager.PrintLogout();
    }

    private void HandleShowBalance()
    {
        var records = _db.Records.Where(r => r.User == _user);
        var sumIncome = records.Where(r => r.Type == RecordType.Income).Sum(r => r.Amount);
        var sumExpenditure = records.Where(r => r.Type == RecordType.Expenditure).Sum(r => r.Amount);
        
        _outputManager.PrintCurrentBalance(sumIncome - sumExpenditure);
    }

    private void HandleListRecords()
    {
        _outputManager.ListRecords(_db.Records.Include(r=>r.RecordCategories).Where(r=>r.User==_user));
    }

    private void HandleDeleteRecord()
    {
        var records = _db.Records.Where(r=>r.User==_user).ToList();
        
        _outputManager.PrintSelectIDToDelete();
        var id = _inputManager.ParseId(records.Select(r=>r.Id).ToList());
        var recordToDelete = records.Find(r=>r.Id == id);
        
        //begin transaction to prevent inconsistencies
        _db.BeginTransaction();
        try
        {
            //remove intermediate table first
            var intermediateTables = _db.RecordCategories.Where(c => c.Record == recordToDelete);
            _db.RecordCategories.RemoveRange(intermediateTables);
            _db.Records.Remove(recordToDelete!);
        }
        catch
        {
            _db.RollbackTransaction();
            _outputManager.PrintErrorDeleting();
            return;
        }
        _db.CommitTransaction();
        _db.SaveChangesAsync();
        
        _outputManager.PrintSuccess();
    }   
    

    private void HandleEditRecord()
    {
        var records = _db.Records.Where(r => r.User == _user).ToList();
            
        _outputManager.PrintSelectId();
        var selectedId = _inputManager.ParseId(records.Select(r => r.Id).ToList());

        var selectedRecord = records.Find(r => r.Id == selectedId)!;
        _outputManager.PrintRecordInfo(selectedRecord);
        
        _outputManager.PrintAddRecordSelectName();
        var newName = _inputManager.ParseString();
        
        _outputManager.PrintAddRecordSelectDate();
        var newDate = (DateTime)_inputManager.ParseDate()!;
        
        _outputManager.PrintAddRecordSelectAmount();
        var newAmount = float.Abs(_inputManager.ParseDecimal());

        selectedRecord.Name = newName;
        selectedRecord.Date = newDate;
        selectedRecord.Amount = newAmount;

        _db.SaveChangesAsync();

        _outputManager.PrintSuccess();

    }

    private void HandleAddRecord()
    {
        var cats = _db.Categories.ToList();
        
        _outputManager.PrintSelectRecordType();
        var recordType = (RecordType)_inputManager.ParseRecordType()!;
        _outputManager.PrintAddRecordSelectName();
        var name = _inputManager.ParseString();
        _outputManager.PrintAddRecordSelectCategory();
        _outputManager.ListCategories(cats);
        var catIds = _inputManager.ParseIds(cats.Select(c=>c.Id).ToList());
        var selectedCats = cats.FindAll(c => catIds.Contains(c.Id)).ToList();
        _outputManager.PrintAddRecordSelectDate();
        var date = (DateTime)_inputManager.ParseDate()!;
        
        _outputManager.PrintAddRecordSelectAmount();
        var amount = float.Abs(_inputManager.ParseDecimal());
        
        //build the record
        var newRecord = new Record
        {
            Name = name,
            Type = recordType,
            User = _user!,
            UserId = _user!.Id,
            Date = date,
            Amount = amount
        };
        
        //open transaction to prevent inconsistency
        _db.BeginTransaction();

        try
        {
            _db.Records.Add(newRecord);
            
            //create intermediate table to add categories
            selectedCats.ForEach(c =>
            {
                _db.RecordCategories.Add(new RecordCategory
                {
                    Category = c,
                    CategoryId = c.Id,
                    Record = newRecord,
                    RecordId = newRecord.Id
                });
            });
        }
        catch
        {
            _db.RollbackTransaction();
            _outputManager.PrintErrorCreating();
            return;
        }
        
        //save changes if all ok
        _db.CommitTransaction();
        _db.SaveChangesAsync();
  
        _outputManager.PrintSuccess();
    }

    private void HandleLogin()
    {
        _outputManager.PrintLoginName();
        var username = _inputManager.ParseString();
        _outputManager.PrintLoginPassword();
        var password = _inputManager.ReadPassword();
        
        //get user by username
        var user = (from u in _db.Users
            where u.Username == username
            select u).FirstOrDefault();

        if (user == null)
        {
            _outputManager.PrintSignupError();
            return;
        }

        if (PasswordHasher.VerifyPassword(user.PasswordHash, password))
        {
            _user = user;
            _outputManager.PrintLoginSuccess();
        }
        else
        {
            _outputManager.PrintSignupError();
        }


    }

    private void HandleRegister()
    {
        _outputManager.PrintSignupName();
        var username = _inputManager.ParseString();
        _outputManager.PrintSignupPassword();
        var password = _inputManager.ParseString();

        var newUser = new User
        {
            PasswordHash = PasswordHasher.HashPassword(password),
            Username = username
        };

        _db.Users.Add(newUser);
        _db.SaveChangesAsync();
        
        _outputManager.PrintSignupSuccess();
        
        _user = newUser;
        _outputManager.PrintLoginSuccess();
    }

    private void CreateDefaultCategoriesIfNeeded()
    {
        //get default categories that already exist
        var existingDefault = _db.Categories
            .Where(c => _defaultCategories.Contains(c.Name))
            .ToList();
        var existingDefaultNames = existingDefault.Select(c => c.Name).ToList();

        var catsToCreate = _defaultCategories
            .Where(name => !existingDefaultNames.Contains(name))
            .ToList();
        var defaultToAdd = catsToCreate
            .Select(name => new Category { Name = name, Default = true })
            .ToList();
        
        _db.Categories.AddRange(defaultToAdd);
        existingDefault.ForEach(c => c.Default = true);

        _db.SaveChangesAsync();
    }

    private void HandleFilterRecords()
    {
        var filteredRows = _db.Records.Include(r=>r.RecordCategories).ToList();
        
        _outputManager.PrintFilterSelectType();
        var filterType = _inputManager.ParseRecordType(allowEmpty:true);
        if (filterType != null)
        {
            filteredRows = filteredRows.Where(r => r.Type == filterType).ToList();
        }
        
        _outputManager.PrintFilterSelectCategories();
        HandleListCategories();
        
        var availableCatIds = _db.Categories.Select(c => c.Id).ToList();
        var filterCatIds = _inputManager.ParseIds(availableCatIds, true);

        if (filterCatIds.Count > 0)
        {
            filteredRows = filteredRows
                .Where(r => r.RecordCategories.Any(rc => filterCatIds.Contains(rc.CategoryId))).ToList();
        }
        
        _outputManager.PrintFilterSelectDateFrom();
        var filterDateFrom = _inputManager.ParseDate(allowEmpty:true);
        if (filterDateFrom is not null)
        {
            filteredRows = filteredRows.Where(r => r.Date > filterDateFrom).ToList();
        }
        
        _outputManager.PrintFilterSelectDateTo();;
        var filterDateTo = _inputManager.ParseDate(allowEmpty:true);
        if (filterDateTo is not null)
        {
            filteredRows = filteredRows.Where(r => r.Date < filterDateTo).ToList();
        }
        
        _outputManager.ListRecords(filteredRows);
    }

    private async void HandleImportData()
    {
        _outputManager.PrintImportSelectPath();
        var filePath = _inputManager.ParseString();
        
        // Ensure the file exists
        if (!File.Exists(filePath))
        {
            _outputManager.PrintImportFileDoesNotExistError();
            return;
        }
        
        //validate data
        try
        {
            await ImportFileManager.ValidateFile(
                filePath, 
                _db.Categories.Select(c=>c.Name).ToList());
        }
        catch (InvalidFileException)
        {
            _outputManager.PrintFileDataInvalid();
            return;
        }

        //parse records from file
        try
        {
            _db.BeginTransaction();
            
            var fileData = await ImportFileManager.ParseFile(filePath);
            fileData.ForEach(row =>
            {
                //set user
                row.record.User = _user;
                row.record.UserId = _user.Id;
            
                _db.Records.Add(row.record);
            
                //get categories from list
                var categories = _db.Categories.Where(c=>row.categories.Contains(c.Name)).ToList();
                categories.ForEach(c =>
                {
                    _db.RecordCategories.Add(new RecordCategory
                    {
                        Category = c,
                        CategoryId = c.Id,
                        Record = row.record,
                        RecordId = row.record.Id
                    });
                });
            });
        
            _db.CommitTransaction();
            await _db.SaveChangesAsync();
            _outputManager.PrintImportAmountImported(fileData.Count);
        }
        catch (Exception)
        {
            _outputManager.PrintFileImportException();
            _db.RollbackTransaction();
            return;
        }

    }

    private void HandleExportData()
    {
        _outputManager.PrintExportSelectFileName();
        var fileName = _inputManager.ParseString();
        
        // Define the file path relative to the project directory
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Export", fileName);
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        var data = _db.Records.Include(r => r.RecordCategories);

        var serializer = new OutputCsvSerializer();
        serializer.SerializeRecords(data.ToList(), filePath);


    }

    private void HandleGenerateStatistics()
    {
        var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Statistics");
        var directory = Path.GetDirectoryName(dirPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        //sum per day
        StatisticsManager.PlotSumPerDay(_db.Records.OrderBy(r=>r.Date).ToList());
        
        // bar chart of income by day
        StatisticsManager.PlotSumAmountPerDay(
            _db.Records.OrderBy(r=>r.Date).Where(r=>r.Type==RecordType.Income).ToList(), 
            Path.Combine(dirPath, "IncomeDailyBars.png")
            );
        // bar chars of expenditure by day
        // bar chart of income by day
        StatisticsManager.PlotSumAmountPerDay(
            _db.Records.OrderBy(r=>r.Date).Where(r=>r.Type==RecordType.Expenditure).ToList(), 
            Path.Combine(dirPath, "ExpenditureDailyBars.png")
        );
    }
    
}