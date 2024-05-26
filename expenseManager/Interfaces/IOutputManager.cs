using expenseManager.Models;

namespace expenseManager.Interfaces;

public interface IOutputManager
{
    public void PrintIntro();
    public void PrintUnsignedMenu();
    public void PrintInternalMenu();
    public void PrintSignupName();
    public void PrintSignupPassword();
    public void PrintSignupError();
    public void PrintSignupSuccess();
    public void PrintLoginName();
    public void PrintLoginPassword();
    public void PrintLoginError();
    public void PrintLoginSuccess();
    public void PrintLogout();

    public void PrintNewCategoryName();
    public void PrintNewCategorySuccess();
    public void PrintEditCategoryId();
    public void PrintEditCategoryName();
    public void PrintEditCategorySuccess();
    public void PrintDeleteCategoryId();
    public void PrintDeleteCategorySuccess();
    public void PrintCategoryNameInvalid();

    public void PrintCategoryUsedError();

    public void PrintCurrentBalance(float balance);

    public void PrintSelectIDToDelete();
    public void PrintErrorDeleting();
    

    public void PrintRecordInfo(Record record);
    public void PrintCategoryInfo(Category category);

    public void PrintAddRecordSelectName();
    public void PrintAddRecordSelectCategory();
    public void PrintAddRecordSelectDate();
    public void PrintAddRecordSelectAmount();
    
    public void ListCategories(IEnumerable<Category> categories);
    public void ListRecords(IEnumerable<Record> records);
    
    public void PrintInputError();
    public void PrintInvalidId();

    public void PrintSuccess();

    public void PrintSelectId();

    public void PrintSelectRecordType();

    public void PrintErrorCreating();

    public void PrintFilterSelectType();
    public void PrintFilterSelectCategories();
    public void PrintFilterSelectDateFrom();
    public void PrintFilterSelectDateTo();

    public void PrintImportSelectPath();
    public void PrintImportFileDoesNotExistError();
    public void PrintImportAmountImported(int amount);
    public void PrintExportSelectFileName();

    public void PrintFileDataInvalid();

    public void PrintFileImportException();

}