namespace TowelBorrowing.Data.Models;

public class AppSetting
{
	private AppSetting()
	{
		
	}
	public AppSetting(string key, string value)
	{
		Key = key;
		Value = value;
	}

	public string Key { get; private set; } = string.Empty;
	public string Value { get; set; } = string.Empty;

}
