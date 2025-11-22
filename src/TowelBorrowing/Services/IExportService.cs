using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Services;

public interface IExportService
{
	void ExportBorrowInfo(List<BorrowRecord> borrowRecords);
}
