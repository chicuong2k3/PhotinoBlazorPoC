using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Data.Configs;

internal class GuestCardConfiguration : IEntityTypeConfiguration<GuestCard>
{
	public void Configure(EntityTypeBuilder<GuestCard> builder)
	{
		builder.HasKey(g => g.CardNumber);

		builder.Property(g => g.CardNumber)
			   .IsRequired()
			   .HasMaxLength(100);

		builder.Property(g => g.HolderName)
			   .IsRequired()
			   .HasMaxLength(250);

		builder.Property(g => g.Building)
			   .IsRequired()
			   .HasMaxLength(4);

		builder.Property(g => g.Floor)
			   .IsRequired()
			   .HasMaxLength(3);

		builder.Property(g => g.RoomNo)
			   .IsRequired()
			   .HasMaxLength(10);

		builder.HasMany(g => g.Logs)
			   .WithOne(l => l.GuestCard)
			   .HasForeignKey(l => l.GuestCardNumber)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}
