using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Services;

public interface IExportService
{
	Task ExportBorrowInfo(List<BorrowRecord> borrowRecords);
}
