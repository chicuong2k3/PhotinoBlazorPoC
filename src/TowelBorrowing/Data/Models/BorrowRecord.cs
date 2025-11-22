using FluentResults;
using TowelBorrowing.Models;

namespace TowelBorrowing.Data.Models;

public class BorrowRecord
{
	public int Id { get; set; }
	public string GuestCardNumber { get; private set; } = string.Empty;
	public GuestCard GuestCard { get; private set; } = default!;
	public int BorrowQuantity { get; private set; }
	public int ReturnQuantity { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public GuestCardOcrStatus Status { get; set; }

	private BorrowRecord() { }
	public BorrowRecord(string guestCardNumber)
	{
		GuestCardNumber = guestCardNumber;
		BorrowQuantity = 0;
		ReturnQuantity = 0;
		CreatedAt = DateTime.Now;
		Status = GuestCardOcrStatus.Processed;
	}

	public Result Borrow(int quantity)
	{
		if (quantity < 0)
			return Result.Fail("Số lượng không hợp lệ");

		BorrowQuantity += quantity;

		return Result.Ok();
	}

	public Result Return(int quantity)
	{
		if (quantity < 0)
			return Result.Fail("Số lượng không hợp lệ");


		if (ReturnQuantity + quantity > BorrowQuantity)
			return Result.Fail("Không được trả quá số lượng mượn");

		ReturnQuantity += quantity;

		return Result.Ok();
	}
}

