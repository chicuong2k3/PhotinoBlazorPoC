using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TowelBorrowing.Data;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Services.Impls;

internal class ExportService : IExportService
{
	private readonly AppDbContext _dbContext;

	public ExportService(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	public async Task ExportBorrowInfo(List<BorrowRecord> borrowRecords)
	{
		using var workbook = new XLWorkbook();
		var ws = workbook.Worksheets.Add($"Export_{DateTime.Now.Date.ToString("dd-MM-yyyy")}");

		ws.Cell(1, 1).Value = "Guest Card";
		ws.Cell(1, 2).Value = "Holder Name";
		ws.Cell(1, 3).Value = "Bldg";
		ws.Cell(1, 4).Value = "Số mượn";
		ws.Cell(1, 5).Value = "Số trả";
		ws.Cell(1, 6).Value = "Còn thiếu";

		ws.Range(1, 1, 1, 7).Style.Font.SetBold();


		int row = 2;
		foreach (var r in borrowRecords)
		{
			if (r.BorrowQuantity - r.ReturnQuantity <= 0)
				continue;

			var building = r.GuestCard?.Building switch
			{
				"1" => "A",
				"2" => "B",
				"Villa" => "Villa",
				_ => ""
			};

			ws.Cell(row, 1).Value = r.GuestCardNumber;
			ws.Cell(row, 2).Value = r.GuestCard?.HolderName ?? "";
			ws.Cell(row, 3).Value = building;
			ws.Cell(row, 4).Value = r.BorrowQuantity;
			ws.Cell(row, 5).Value = r.ReturnQuantity;
			ws.Cell(row, 6).Value = r.BorrowQuantity - r.ReturnQuantity;
			row++;
		}

		ws.Columns().AdjustToContents();

		var fileName = $"BorrowInfo_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.xlsx";

		var folderName = (await _dbContext.AppSettings
							.FirstOrDefaultAsync(x => x.Key == Constants.ExportFolderName))?.Value;
		var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), folderName ?? "MuonKhanExport");

		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		var filePath = Path.Combine(
			folderPath,
			fileName
		);

		if (File.Exists(filePath))
			File.Delete(filePath);

		workbook.SaveAs(filePath);
	}
}
