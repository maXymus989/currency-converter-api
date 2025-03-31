var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<ExchangeRateService>();
builder.Services.AddSingleton<ExchangeRateService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<ExchangeRateService>());

builder.Services.AddControllers();
var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
