var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MVFC_ChaosEngineering_Playground_Api>("chaos-api");

await builder.Build().RunAsync().ConfigureAwait(false);
