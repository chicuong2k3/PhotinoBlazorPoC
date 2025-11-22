using TowelBorrowing.Models;

namespace TowelBorrowing.Services;

public interface IGuestCardService
{
	Task<GuestCardOcrResult> ExtractGuestCardAsync(int maxRetries = 3);
}
