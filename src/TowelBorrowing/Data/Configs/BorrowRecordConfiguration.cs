using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Data.Configs;

internal class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
{
	public void Configure(EntityTypeBuilder<BorrowRecord> builder)
	{
		builder.HasKey(g => g.Id);

		builder.Property(g => g.GuestCardNumber)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(g => g.Status)
			   .HasConversion<string>()
			   .IsRequired();

		builder.Property(g => g.CreatedAt)
			   .HasDefaultValueSql("CURRENT_TIMESTAMP")
			   .IsRequired();


		builder.HasOne(g => g.GuestCard)
			   .WithMany(c => c.Logs)
			   .HasForeignKey(g => g.GuestCardNumber)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}
