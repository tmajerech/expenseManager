using System.Text;
using expenseManager.Interfaces;
using expenseManager.Models;
using System.Threading.Tasks;
using System.Linq;
namespace expenseManager.IO;

public class OutputCsvSerializer:ISerializer
{
    public async void SerializeRecords(List<Record> records, string filePath)
    {
        // Use StreamWriter to write to the file asynchronously
        await using StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
        // Write the header
        await writer.WriteLineAsync("ID,Type,Name,Amount,Categories,Date");
        
        // Write each person's data
        var tasks = records.Select(r => ProcessRow(writer: writer, r));
        await Task.WhenAll(tasks);
    }

    private async Task ProcessRow(StreamWriter writer, Record r)
    {
        var categoriesNames = r.RecordCategories?.Select(rc => rc.Category)?.Select(c => c.Name).ToList() ?? []; 
        var line = $"{r.Id},{r.Type},{r.Name},{r.Amount},{string.Join(";", categoriesNames)},{r.Date.ToString("yyyy-MM-dd")}";
        await writer.WriteLineAsync(line);
    }
}