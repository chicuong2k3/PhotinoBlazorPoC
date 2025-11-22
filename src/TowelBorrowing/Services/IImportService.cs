namespace TowelBorrowing.Services;

public interface IImportService
{
	List<List<string>> ReadExcel(byte[] bytes);
}
