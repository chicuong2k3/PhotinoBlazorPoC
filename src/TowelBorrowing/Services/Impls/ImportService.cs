using ClosedXML.Excel;

namespace TowelBorrowing.Services.Impls;

internal class ImportService : IImportService
{

	public List<List<string>> ReadExcel(byte[] bytes)
	{
		using var ms = new MemoryStream(bytes);
		using var workbook = new XLWorkbook(ms);

		var ws = workbook.Worksheets.First();
		var result = new List<List<string>>();

		foreach (var row in ws.RowsUsed())
		{
			result.Add(row
				.Cells()
				.Select(c => c.GetString())
				.ToList());
		}

		return result;
	}
}
