using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TowelBorrowing.Data.Models;

namespace TowelBorrowing.Data.Configs;

internal class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
{
	public void Configure(EntityTypeBuilder<AppSetting> builder)
	{
		builder.HasKey(x => x.Key);

		builder.Property(x => x.Value)
			.IsRequired();
	}
}
