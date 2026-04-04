namespace MVFC.ChaosEngineering.Tests.Fixture;

internal sealed class ProjectAppHost() : DistributedApplicationFactory(typeof(MVFC_ChaosEngineering_Playground_Api))
{
    protected override void OnBuilderCreating(DistributedApplicationOptions applicationOptions, HostApplicationBuilderSettings hostOptions)
    {
        applicationOptions.AllowUnsecuredTransport = true;
        hostOptions.Args = ["--environment=Development"];
    }
}
