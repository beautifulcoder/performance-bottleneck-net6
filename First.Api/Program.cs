var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(_ => new HttpClient(
  new SocketsHttpHandler
  {
    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
  })
{
  BaseAddress = new Uri("http://localhost:5176")
});

var app = builder.Build();

app.MapGet("/", async (HttpClient client) =>
{
  var result = new List<List<WeatherForecast>?>();

  for (var i = 0; i < 100; i++)
  {
    result.Add(
      await client.GetFromJsonAsync<List<WeatherForecast>>(
        "/weatherForecast"));
  }

  return result[Random.Shared.Next(0, 100)];
});

app.MapGet("/optimization-bad", async (HttpClient client) =>
  (await Task.WhenAll(
    Enumerable
      .Range(0, 100)
      .Select(_ =>
        client.GetFromJsonAsync<List<WeatherForecast>>(
          "/weatherForecast")
      )
    )
  )
  .ToArray()[Random.Shared.Next(0, 100)]);

var sem = new SemaphoreSlim(10);

app.MapGet("/optimization-good", async (HttpClient client) =>
  (await Task.WhenAll(
    Enumerable
      .Range(0, 100)
      .Select(async _ =>
      {
        try
        {
          await sem.WaitAsync();
          return await client.GetFromJsonAsync<List<WeatherForecast>>(
            "/weatherForecast");
        }
        finally
        {
          sem.Release();
        }
      })
    )
  )
  .ToArray()[Random.Shared.Next(0, 100)]);

app.Run();

public record WeatherForecast(
  DateTime Date,
  int TemperatureC,
  string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
