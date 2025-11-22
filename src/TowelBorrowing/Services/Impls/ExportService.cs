using ClosedXML.Excel;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Services.Impls;

internal class ExportService : IExportService
{
	public void ExportBorrowInfo(List<BorrowRecord> borrowRecords)
	{
		using var workbook = new XLWorkbook();
		var ws = workbook.Worksheets.Add($"Export_{DateTime.Now.Date.ToString("dd-MM-yyyy")}");

		ws.Cell(1, 1).Value = "Guest Card";
		ws.Cell(1, 2).Value = "Holder Name";
		ws.Cell(1, 3).Value = "Bldg";
		ws.Cell(1, 4).Value = "Số mượn";
		ws.Cell(1, 5).Value = "Số trả";

		ws.Range(1, 1, 1, 7).Style.Font.SetBold();


		int row = 2;
		foreach (var r in borrowRecords)
		{
			var building = r.GuestCard?.Building switch
			{
				"1" => "A",
				"2" => "B",
				_ => ""
			};

			ws.Cell(row, 1).Value = r.GuestCardNumber;
			ws.Cell(row, 2).Value = r.GuestCard?.HolderName ?? "";
			ws.Cell(row, 3).Value = building;
			ws.Cell(row, 4).Value = r.BorrowQuantity;
			ws.Cell(row, 5).Value = r.ReturnQuantity;
			row++;
		}

		ws.Columns().AdjustToContents();

		var filePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
			$"BorrowInfo_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx"
		);

		workbook.SaveAs(filePath);
	}
}
