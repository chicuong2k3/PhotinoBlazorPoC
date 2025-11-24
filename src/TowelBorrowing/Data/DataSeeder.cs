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

			var rooms = new List<GuestCard>();
			for (int i = 2101; i <= 2131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 2201; i <= 2233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 2301; i <= 2323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 2401; i <= 2415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3101; i <= 3131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3201; i <= 3233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3301; i <= 3323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3401; i <= 3415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4101; i <= 4131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4201; i <= 4233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4301; i <= 4323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4401; i <= 4415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5101; i <= 5131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5201; i <= 5233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5301; i <= 5323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5401; i <= 5415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 6101; i <= 6131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}
			for (int i = 6201; i <= 6233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}
			for (int i = 6301; i <= 6323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}
			for (int i = 6401; i <= 6415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}

			for (int i = 7101; i <= 7131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}
			for (int i = 7201; i <= 7233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}
			for (int i = 7301; i <= 7323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}
			for (int i = 7401; i <= 7415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}


			//8

			for (int i = 8101; i <= 8131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}
			for (int i = 8201; i <= 8233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}
			for (int i = 8301; i <= 8323; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}
			for (int i = 8401; i <= 8415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "1",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}



			// Building B
			for (int i = 2101; i <= 2131; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 2201; i <= 2233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 2301; i <= 2321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 2401; i <= 2415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "2",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3101; i <= 3125; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3201; i <= 3233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3301; i <= 3321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 3401; i <= 3415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "3",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4101; i <= 4125; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4201; i <= 4233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4301; i <= 4321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			for (int i = 4401; i <= 4415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "4",
					RoomNo = i.ToString()
				});
			}

			// 5
			for (int i = 5101; i <= 5125; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5201; i <= 5233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5301; i <= 5321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			for (int i = 5401; i <= 5415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "5",
					RoomNo = i.ToString()
				});
			}

			// 6
			for (int i = 6101; i <= 6125; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}

			for (int i = 6201; i <= 6233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}

			for (int i = 6301; i <= 6321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}

			for (int i = 6401; i <= 6415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "6",
					RoomNo = i.ToString()
				});
			}

			// 7
			for (int i = 7101; i <= 7125; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}

			for (int i = 7201; i <= 7233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}

			for (int i = 7301; i <= 7321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}

			for (int i = 7401; i <= 7415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "7",
					RoomNo = i.ToString()
				});
			}

			// 8
			for (int i = 8101; i <= 8125; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}

			for (int i = 8201; i <= 8233; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}

			for (int i = 8301; i <= 8321; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}

			for (int i = 8401; i <= 8415; i++)
			{
				rooms.Add(new GuestCard()
				{
					CardNumber = Guid.NewGuid().ToString(),
					HolderName = i.ToString(),
					Building = "2",
					Floor = "8",
					RoomNo = i.ToString()
				});
			}

			dbContext.GuestCards.AddRange(rooms);

			dbContext.SaveChanges();
		}

		if (!dbContext.AppSettings.Any())
		{
			dbContext.AppSettings.Add(new AppSetting(Constants.OcrApiKey, "my-secret-api-key"));
			dbContext.AppSettings.Add(new AppSetting(Constants.OcrApiUrl, "http://localhost:10000"));
			dbContext.AppSettings.Add(new AppSetting(Constants.RoomManagementAppName, "Photos"));
			dbContext.AppSettings.Add(new AppSetting(Constants.ExportFolderName, "MuonKhanExport"));
			dbContext.SaveChanges();
		}
 	}
}
