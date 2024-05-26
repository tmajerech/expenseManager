using System.Text;
using expenseManager.Enums;
using expenseManager.Interfaces;

namespace expenseManager.IO;



public class ConsoleInputManager : IInputManager
{
    private IOutputManager _outputManager;

    private bool ValidateString(string value)
    {
        value = value.Trim();
        return value.Length > 0;
    }

    public ConsoleInputManager(IOutputManager outputManager)
    {
        _outputManager = outputManager;
    }

    public UnsignedMenuOptions ParseUnsignedMenuSelection()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }

            if (int.TryParse(userInput, out var userChoice))
            {
                switch (userChoice)
                {
                    case 1:
                        return UnsignedMenuOptions.LogIn;
                    case 2:
                        return UnsignedMenuOptions.Register;
                    default:
                        _outputManager.PrintInputError();
                        continue;
                }
            }
            else
            {
                _outputManager.PrintInputError();
                continue;
            }
        }
    }

    public InternalMenuOptions ParseInternalMenuSelection()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }
            
            if (int.TryParse(userInput, out var userChoice))
            {
                switch (userChoice)
                {
                    case 1:
                        return InternalMenuOptions.AddRecord;
                    case 2:
                        return InternalMenuOptions.ListRecords;
                    case 3:
                        return InternalMenuOptions.EditRecord;
                    case 4:
                        return InternalMenuOptions.DeleteRecord;
                    case 5:
                        return InternalMenuOptions.ShowBalance;
                    case 6:
                        return InternalMenuOptions.Logout;
                    case 7:
                        return InternalMenuOptions.AddCategory;
                    case 8:
                        return InternalMenuOptions.EditCategory;
                    case 9:
                        return InternalMenuOptions.DeleteCategory;
                    case 10:
                        return InternalMenuOptions.ListCategories;
                    case 11:
                        return InternalMenuOptions.FilterRecords;
                    case 12:
                        return InternalMenuOptions.ImportData;
                    case 13:
                        return InternalMenuOptions.ExportData;
                    case 14:
                        return InternalMenuOptions.GenerateStatistics;
                    default:
                        _outputManager.PrintInputError();
                        continue;
                }
            }
            else
            {
                _outputManager.PrintInputError();
                continue;
            }
        }
    }

    public string ParseRecordName()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }

            if (!ValidateString(userInput))
            {
                _outputManager.PrintInputError();
                continue;
            }
            else
            {
                return userInput.Trim();
            }
        }
    }

    public List<int> ParseIds(List<int> availableIds, bool allowEmpty)
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null or "")
            {
                if (allowEmpty)
                {
                    return [];
                }
                else
                {
                    continue;
                }
            }
            
            
            //split values and parse all
            var values = userInput.Split(",").Select(s => s.Trim());
            List<int> returnValues = [];
            var hasError = false;
            foreach (var value in values)
            {
                if (int.TryParse(value, out var id) && availableIds.Contains(id))
                {
                    returnValues.Add(id);
                }
                else
                {
                    hasError = true;
                    break;
                }
            }

            if (!hasError) return returnValues;
            
            _outputManager.PrintInvalidId();
        }
    }

    public string ParseString()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }

            if (!ValidateString(userInput))
            {
                _outputManager.PrintInputError();
                continue;
            }
            else
            {
                return userInput.Trim();
            }
        }
    }

    public float ParseDecimal()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }

            if (float.TryParse(userInput, out var parsedValue))
            {
                return parsedValue;
            }
            else
            {
                _outputManager.PrintInputError();
                continue;
            }
        }
    }

    public int ParseInt()
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }

            if (int.TryParse(userInput, out var parsedValue))
            {
                return parsedValue;
            }
            else
            {
                _outputManager.PrintInputError();
                continue;
            }
        }
    }
    
    public int ParseId(List<int> availableIds)
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput is null)
            {
                continue;
            }

            if (int.TryParse(userInput, out var parsedValue) && availableIds.Contains(parsedValue))
            {
                return parsedValue;
            }
            else
            {
                _outputManager.PrintInvalidId();
                continue;
            }
        }
    }

    public DateTime? ParseDate(bool allowEmpty=false)
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput=="")
            {
                if (allowEmpty)
                {
                    return null;
                }
                else
                {
                    continue;
                }
            }

            if (DateTime.TryParse(userInput, out var parsedValue))
            {
                return parsedValue;
            }
            else
            {
                _outputManager.PrintInputError();
                continue;
            }
        }
    }

    public RecordType? ParseRecordType(bool allowEmpty=false)
    {
        while (true)
        {
            var userInput = Console.ReadLine();
            if (userInput=="")
            {
                if (allowEmpty)
                {
                    return null;
                }
                else
                {
                    continue;
                }
            }
            

            if (Enum.TryParse<RecordType>(userInput, out var parsedValue))
            {
                if (Enum.IsDefined(typeof(RecordType), parsedValue))
                {
                    return parsedValue;
                }
                else
                {
                    _outputManager.PrintInputError();
                    continue;
                }
            }

        }
    }

    public string ReadPassword()
    {
        StringBuilder passwordBuilder = new StringBuilder();
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (passwordBuilder.Length > 0)
                {
                    passwordBuilder.Length--;
                    Console.Write("\b \b"); // Erase the last '*' character
                }
            }
            else
            {
                passwordBuilder.Append(keyInfo.KeyChar);
                Console.Write('*');
            }
        }

        Console.WriteLine();
        return passwordBuilder.ToString();
    }
}