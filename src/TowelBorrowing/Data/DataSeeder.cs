using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Data;

public static class DataSeeder
{
	public static void SeedData(this IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		if (dbContext.Database.GetPendingMigrations().Any())
		{
			dbContext.Database.Migrate();
		}

		var villaRoomNos = new List<int>()
		{
			6801, 6802, 6803, 6804, 6805, 6806, 6807, 6808, 6809, 6810, 6811, 6812,
			6814, 6815, 6815, 6816, 6817, 6818, 6819, 6820, 6821, 6822
		};

		if (!dbContext.GuestCards.Any())
		{
			var villas = new List<GuestCard>();
			foreach (var villaRoomNo in villaRoomNos)
			{
				villas.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = villaRoomNo.ToString(),
					Building = "Villa",
					Floor = "",
					RoomNo = villaRoomNo.ToString()
				});
			}

			dbContext.GuestCards.AddRange(villas);
			dbContext.SaveChanges();
		}

		if (!dbContext.AppSettings.Any())
		{
			dbContext.AppSettings.Add(new AppSetting(Constants.OcrApiKey, "my-secret-api-key"));
			dbContext.AppSettings.Add(new AppSetting(Constants.OcrApiUrl, "http://localhost:10000"));
			dbContext.AppSettings.Add(new AppSetting(Constants.RoomManagementAppName, "Photos"));
			dbContext.SaveChanges();
		}
 	}
}
