using System.Runtime.InteropServices.JavaScript;
using expenseManager.Enums;

namespace expenseManager.Interfaces;

public interface IInputManager
{
    public UnsignedMenuOptions ParseUnsignedMenuSelection();
    public InternalMenuOptions ParseInternalMenuSelection();
    public string ParseRecordName();
    public List<int> ParseIds(List<int> availableIds, bool allowEmpty=false);
    public string ParseString();
    public float ParseDecimal();
    public int ParseInt();
    public int ParseId(List<int> availableIds);
    public DateTime? ParseDate(bool allowEmpty=false);
    public RecordType? ParseRecordType(bool allowEmpty=false);
    public string ReadPassword();

}