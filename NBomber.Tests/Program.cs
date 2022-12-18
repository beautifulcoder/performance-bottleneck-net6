using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;

var step = Step.Create(
  "fetch_first_api",
  clientFactory: HttpClientFactory.Create(),
  execute: async context =>
  {
    var request = Http
      .CreateRequest("GET", "http://localhost:5060/")
      .WithHeader("Accept", "application/json");
    var response = await Http.Send(request, context);

    return response.StatusCode == 200
      ? Response.Ok(
        statusCode: response.StatusCode,
        sizeBytes: response.SizeBytes)
      : Response.Fail();
  });

var scenario = ScenarioBuilder
  .CreateScenario("first_http", step)
  .WithWarmUpDuration(TimeSpan.FromSeconds(5))
  .WithLoadSimulations(
    Simulation.InjectPerSec(rate: 1, during: TimeSpan.FromSeconds(5)),
    Simulation.InjectPerSec(rate: 2, during: TimeSpan.FromSeconds(10)),
    Simulation.InjectPerSec(rate: 3, during: TimeSpan.FromSeconds(15))
  );

NBomberRunner
  .RegisterScenarios(scenario)
  .Run();
