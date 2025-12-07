
using TowelBorrowing.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials()
			.SetIsOriginAllowed(_ => true);
	});
});

var app = builder.Build();


app.UseRouting();
app.UseCors("AllowAll");

app.MapHub<BorrowHub>("/borrowhub");

app.MapGet("/health", () => "OK");

app.Run();
