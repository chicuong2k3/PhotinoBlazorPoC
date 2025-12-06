using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Data.Configs;

internal class MaxClientMonitorConfiguration : IEntityTypeConfiguration<MaxClientMonitor>
{
	public void Configure(EntityTypeBuilder<MaxClientMonitor> builder)
	{
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id)
			.ValueGeneratedNever();

		builder.Property(x => x.RoomNo)
			   .IsRequired()
			   .HasMaxLength(10);

		builder.Property(x => x.Building)
			   .IsRequired()
			   .HasMaxLength(10);
	}
}
