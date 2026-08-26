using System.Reflection;
using System.Reflection.Emit;
using Asm.AspNetCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Tests.Modules;

[Binding]
public class ModulesSteps
{
    private const string ProbePrefix = "Asm.AspNetCore.Tests.Probe.";

    private static readonly string[] FrameworkAssemblies = ["System.", "Microsoft.", "netstandard", "mscorlib", "WindowsBase"];

    private WebApplicationBuilder _builder = null!;
    private WebApplicationBuilder _result = null!;
    private WebApplication _app = null!;
    private Assembly _assembly = null!;
    private IServiceCollection _services = null!;
    private IServiceProvider _provider = null!;
    private Assembly[] _candidates = null!;
    private string _unloadedAssemblyName = null!;

    [Given(@"I have a WebApplicationBuilder")]
    public void GivenIHaveAWebApplicationBuilder()
    {
        _builder = WebApplication.CreateBuilder();
        ResetModuleFlags();
    }

    [Given(@"I have a module service collection")]
    public void GivenIHaveAModuleServiceCollection()
    {
        _services = new ServiceCollection();
        ResetModuleFlags();
    }

    [Given(@"I have registered two modules")]
    public void GivenIHaveRegisteredTwoModules()
    {
        _result = _builder.RegisterModules(() => [new TestModule(), new SecondTestModule()]);
    }

    [When(@"I add a TestModule to the collection")]
    public void WhenIAddATestModuleToTheCollection()
    {
        _services.AddModule<TestModule>();
    }

    [When(@"I add a SecondTestModule to the collection")]
    public void WhenIAddASecondTestModuleToTheCollection()
    {
        _services.AddModule<SecondTestModule>();
    }

    [Then(@"the service provider should resolve (.*) module\(s\)")]
    public void ThenTheServiceProviderShouldResolveModules(int count)
    {
        _provider = _services.BuildServiceProvider();
        Assert.Equal(count, _provider.GetServices<IModule>().Count());
    }

    [Then(@"the module services should be registered in the collection")]
    public void ThenTheModuleServicesShouldBeRegisteredInTheCollection()
    {
        Assert.True(TestModule.ServicesAdded);
    }

    [Then(@"both module endpoints should be mapped")]
    public void ThenBothModuleEndpointsShouldBeMapped()
    {
        Assert.True(TestModule.EndpointsMapped);
        Assert.True(SecondTestModule.EndpointsMapped);
    }

    private static void ResetModuleFlags()
    {
        TestModule.ServicesAdded = false;
        TestModule.EndpointsMapped = false;
        SecondTestModule.ServicesAdded = false;
        SecondTestModule.EndpointsMapped = false;
    }

    [Given(@"I have an assembly with an IModule implementation")]
    public void GivenIHaveAnAssemblyWithAnIModuleImplementation()
    {
        _assembly = typeof(TestModule).Assembly;
    }

    [Given(@"I have registered modules")]
    public void GivenIHaveRegisteredModules()
    {
        _result = _builder.RegisterModules(() => [new TestModule()]);
    }

    [When(@"I call RegisterModules with the assembly")]
    public void WhenICallRegisterModulesWithTheAssembly()
    {
        _result = _builder.RegisterModules(_assembly);
    }

    [When(@"I call RegisterModules with pattern '(.*)'")]
    public void WhenICallRegisterModulesWithPattern(string pattern)
    {
        _result = _builder.RegisterModules(pattern);
    }

    [When(@"I call RegisterModules with no arguments")]
    public void WhenICallRegisterModulesWithNoArguments()
    {
        _result = _builder.RegisterModules();
    }

    [When(@"I call RegisterModules with a marker type")]
    public void WhenICallRegisterModulesWithAMarkerType()
    {
        _result = _builder.RegisterModules<TestModule>();
    }

    [When(@"I get the candidate assemblies for discovery")]
    public void WhenIGetTheCandidateAssembliesForDiscovery()
    {
        _candidates = [.. ModuleDiscovery.GetCandidateAssemblies()];
    }

    [Then(@"the discovered modules should include TestModule and SecondTestModule")]
    public void ThenTheDiscoveredModulesShouldIncludeTestModuleAndSecondTestModule()
    {
        var modules = _builder.Services.BuildServiceProvider().GetServices<IModule>().ToList();

        Assert.Contains(modules, m => m is TestModule);
        Assert.Contains(modules, m => m is SecondTestModule);
    }

    [Then(@"the candidates should include every loaded application assembly")]
    public void ThenTheCandidatesShouldIncludeEveryLoadedApplicationAssembly()
    {
        var loaded = AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsFrameworkAssembly(a.GetName().Name));

        var missing = loaded.Except(_candidates).Select(a => a.GetName().Name).ToList();

        Assert.True(missing.Count == 0, "Not candidates: " + String.Join(", ", missing));
    }

    [Given(@"an assembly is deployed alongside the application but not loaded")]
    public void GivenAnAssemblyIsDeployedAlongsideTheApplicationButNotLoaded()
    {
        // Any probe left behind by an earlier run gets loaded by the first scenario that discovers, so
        // give this one a name unique to the process. That makes "not loaded" true whatever order the
        // scenarios run in. Tidy up the leftovers that are not currently in use while we are here.
        foreach (var stale in Directory.EnumerateFiles(AppContext.BaseDirectory, $"{ProbePrefix}*.dll"))
        {
            try
            {
                File.Delete(stale);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Loaded by an earlier scenario, so it cannot be deleted. It is harmless.
            }
        }

        _unloadedAssemblyName = $"{ProbePrefix}{Environment.ProcessId}";

        PersistedAssemblyBuilder builder = new(new AssemblyName(_unloadedAssemblyName), typeof(object).Assembly);
        builder.DefineDynamicModule(_unloadedAssemblyName).DefineType("Probe.Marker", TypeAttributes.Public | TypeAttributes.Class).CreateType();
        builder.Save(Path.Combine(AppContext.BaseDirectory, $"{_unloadedAssemblyName}.dll"));

        // Precondition: nothing has touched it, so AppDomain.GetAssemblies() cannot see it.
        Assert.DoesNotContain(AppDomain.CurrentDomain.GetAssemblies(), a => a.GetName().Name == _unloadedAssemblyName);
    }

    [Then(@"the candidates should include the assembly that was not loaded")]
    public void ThenTheCandidatesShouldIncludeTheAssemblyThatWasNotLoaded()
    {
        // The whole point of the fix: an assembly sitting alongside the application that nothing has
        // touched is still a discovery candidate. AppDomain.GetAssemblies() alone would never see it.
        Assert.Contains(_candidates, a => a.GetName().Name == _unloadedAssemblyName);
    }

    [Then(@"the candidates should exclude framework assemblies")]
    public void ThenTheCandidatesShouldExcludeFrameworkAssemblies()
    {
        Assert.DoesNotContain(_candidates, a => IsFrameworkAssembly(a.GetName().Name));
    }

    private static bool IsFrameworkAssembly(string? name) =>
        name is null || FrameworkAssemblies.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal));

    [When(@"I call RegisterModules with a module factory")]
    public void WhenICallRegisterModulesWithAModuleFactory()
    {
        _result = _builder.RegisterModules(() => [new TestModule()]);
    }

    [When(@"I build the application and map module endpoints")]
    public void WhenIBuildTheApplicationAndMapModuleEndpoints()
    {
        _app = _builder.Build();
        _app.MapModuleEndpoints();
    }

    [Then(@"the builder should be returned")]
    public void ThenTheBuilderShouldBeReturned()
    {
        Assert.NotNull(_result);
        Assert.Same(_builder, _result);
    }

    [Then(@"the module services should be registered")]
    public void ThenTheModuleServicesShouldBeRegistered()
    {
        Assert.True(TestModule.ServicesAdded);
    }

    [Then(@"the module endpoints should be mapped")]
    public void ThenTheModuleEndpointsShouldBeMapped()
    {
        Assert.True(TestModule.EndpointsMapped);
    }
}

/// <summary>
/// Test implementation of IModule for testing purposes.
/// </summary>
public class TestModule : IModule
{
    public static bool ServicesAdded { get; set; }
    public static bool EndpointsMapped { get; set; }

    public IServiceCollection AddServices(IServiceCollection services)
    {
        ServicesAdded = true;
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        EndpointsMapped = true;
        return endpoints;
    }
}

/// <summary>
/// A second test implementation of IModule, used to verify additive registration and DI resolution.
/// </summary>
public class SecondTestModule : IModule
{
    public static bool ServicesAdded { get; set; }
    public static bool EndpointsMapped { get; set; }

    public IServiceCollection AddServices(IServiceCollection services)
    {
        ServicesAdded = true;
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        EndpointsMapped = true;
        return endpoints;
    }
}
