using Microsoft.EntityFrameworkCore;
using TowelBorrowing.Data.Configs;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
 	{
		
	}
	public DbSet<GuestCard> GuestCards { get; set; } = default!;
	public DbSet<BorrowRecord> BorrowRecords { get; set; } = default!;
	public DbSet<MaxClientMonitor> MaxClientMonitors { get; set; } = default!;
	public DbSet<AppSetting> AppSettings { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new GuestCardConfiguration());
		modelBuilder.ApplyConfiguration(new BorrowRecordConfiguration());
		modelBuilder.ApplyConfiguration(new MaxClientMonitorConfiguration());
		modelBuilder.ApplyConfiguration(new AppSettingConfiguration());
	}
}
