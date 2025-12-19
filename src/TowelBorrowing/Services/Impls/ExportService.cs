using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TowelBorrowing.Data;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Services.Impls;

internal class ExportService : IExportService
{
	private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
	private static readonly TimeZoneInfo _vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

	public ExportService(IDbContextFactory<AppDbContext> dbContextFactory)
	{
		_dbContextFactory = dbContextFactory;
	}
	
	public async Task ExportBorrowInfo(List<BorrowRecord> borrowRecords)
	{
		var vietnamNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _vietnamTimeZone);
		
		using var workbook = new XLWorkbook();
		var ws = workbook.Worksheets.Add($"Export_{vietnamNow.Date.ToString("dd-MM-yyyy")}");

		ws.Cell(1, 1).Value = "Guest Card";
		ws.Cell(1, 2).Value = "Holder Name";
		ws.Cell(1, 3).Value = "Bldg";
		ws.Cell(1, 4).Value = "Số mượn";
		ws.Cell(1, 5).Value = "Số trả";
		ws.Cell(1, 6).Value = "Còn thiếu";

		ws.Range(1, 1, 1, 7).Style.Font.SetBold();

		int borrowSum = 0;
		int returnSum = 0;
		int remainSum = 0;

		int row = 2;
		foreach (var r in borrowRecords)
		{
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

			borrowSum += r.BorrowQuantity;
			returnSum += r.ReturnQuantity;
			remainSum += (r.BorrowQuantity - r.ReturnQuantity);

			row++;
		}

		ws.Cell(row, 4).Value = borrowSum;
		ws.Cell(row, 5).Value = returnSum;
		ws.Cell(row, 6).Value = remainSum;

		ws.Columns().AdjustToContents();

		var fileName = $"BorrowInfo_{vietnamNow:dd-MM-yyyy_HH-mm-ss}.xlsx";

		await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
		var folderName = (await dbContext.AppSettings
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
