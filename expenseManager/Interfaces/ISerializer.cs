using expenseManager.Models;

namespace expenseManager.Interfaces;

public interface ISerializer
{
    public void SerializeRecords(List<Record> records, string filePath);
}