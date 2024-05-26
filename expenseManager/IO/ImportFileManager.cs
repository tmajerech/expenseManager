using System.Globalization;
using expenseManager.Enums;
using expenseManager.Exceptions;
using expenseManager.Models;
using ScottPlot;

namespace expenseManager.IO;

public static class ImportFileManager
{
        public static async Task ValidateFile(string filePath,  IEnumerable<string> categories)
    {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read and discard header line
                await reader.ReadLineAsync();

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine("parsing " + line);
                    // "ID,Type,Name,Amount,Categories,Date"
                    var values = line.Split(",");
                    //check total number of cols
                    if (values.Length != 6)
                    {
                        throw new InvalidFileException();
                    }
                    
                    //check that type is income/expenditure
                    if (!Enum.IsDefined(typeof(RecordType), values[1]))
                    {
                        throw new InvalidFileException();
                    } 
                    
                    // check that name contains some chars
                    if (values[2].Length == 0)
                    {
                        throw new InvalidFileException();
                    }
                    
                    //check that amount is float
                    if (!float.TryParse(values[3], out _))
                    {
                        throw new InvalidFileException();
                    }
                    
                    //check that categories contain at least one
                    if (values[4].Length == 0)
                    {
                        throw new InvalidFileException("WTF");
                    }
                    //check that categories match existing ones
                    if (!values[4].Split(";").ToList().All(categories.Contains))
                    {
                        throw new InvalidFileException();
                    }
                    
                    //check that date is valid
                    if(!DateTime.TryParseExact(
                           values[5],
                           "yyyy-MM-dd",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out _))
                    {
                        throw new InvalidFileException();
                    }
                }
            }
    }

    public static async Task<List<(Record record, List<string> categories)>> ParseFile(string filePath)
    {
        var result = new List<(Record, List<string>)>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            // Read and discard header line
            await reader.ReadLineAsync();

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var values = line.Split(',');
                // "ID,Type,Name,Amount,Categories,Date"
                var catNames = values[4].Split(";");
                var record = new Record
                {
                    Type = (RecordType)Enum.Parse(typeof(RecordType), values[1], true),
                    Name = values[2],
                    Amount = float.Parse(values[3]),
                    Date = DateTime.ParseExact(values[5], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                };
                
                result.Add(new (record, catNames.ToList()));
            }
        }

        return result;
    }
}