var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var summaries = new[]
{
  "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherForecast", async () =>
{
  await Task.Delay(10);
  return Enumerable
    .Range(0, 1000)
    .Select(index =>
      new WeatherForecast
      (
        DateTime.Now.AddDays(index),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
      )
    )
    .ToArray()[..5];
});

app.Run();

public record WeatherForecast(
  DateTime Date,
  int TemperatureC,
  string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
