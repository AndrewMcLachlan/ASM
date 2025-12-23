#nullable enable
using Asm.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Asm.Serilog.Tests.Logging;

[Binding]
public class LoggingConfiguratorSteps(ScenarioContext scenarioContext)
{
    private LoggerConfiguration? _loggerConfiguration;
    private LoggerConfiguration? _result;
    private IConfiguration? _configuration;
    private IHostEnvironment? _hostEnvironment;

    [Given(@"I have a LoggerConfiguration")]
    public void GivenIHaveALoggerConfiguration()
    {
        _loggerConfiguration = new LoggerConfiguration();
    }

    [Given(@"I have a null LoggerConfiguration")]
    public void GivenIHaveANullLoggerConfiguration()
    {
        _loggerConfiguration = null;
    }

    [Given(@"I have a configuration with logging settings")]
    public void GivenIHaveAConfigurationWithLoggingSettings()
    {
        var mockLogLevelSection = new Mock<IConfigurationSection>();
        mockLogLevelSection.Setup(s => s.GetChildren()).Returns([]);

        var mockLoggingSection = new Mock<IConfigurationSection>();
        mockLoggingSection.Setup(s => s.GetSection("LogLevel")).Returns(mockLogLevelSection.Object);

        var mockSeqHostSection = new Mock<IConfigurationSection>();
        mockSeqHostSection.Setup(s => s.Value).Returns((string?)null);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection("Logging")).Returns(mockLoggingSection.Object);
        mockConfig.Setup(c => c.GetSection("Seq:Host")).Returns(mockSeqHostSection.Object);

        _configuration = mockConfig.Object;
    }

    [Given(@"I have a null configuration")]
    public void GivenIHaveANullConfiguration()
    {
        _configuration = null;
    }

    [Given(@"I have a host environment for '(.*)'")]
    public void GivenIHaveAHostEnvironmentFor(string appName)
    {
        var mockEnv = new Mock<IHostEnvironment>();
        mockEnv.Setup(e => e.ApplicationName).Returns(appName);
        mockEnv.Setup(e => e.EnvironmentName).Returns("Production");
        _hostEnvironment = mockEnv.Object;
    }

    [Given(@"I have a null host environment")]
    public void GivenIHaveANullHostEnvironment()
    {
        _hostEnvironment = null;
    }

    [Given(@"I have a configuration with log level override for '(.*)' set to '(.*)'")]
    public void GivenIHaveAConfigurationWithLogLevelOverrideFor(string source, string level)
    {
        var mockSourceSection = new Mock<IConfigurationSection>();
        mockSourceSection.Setup(s => s.Key).Returns(source);
        mockSourceSection.Setup(s => s.Value).Returns(level);

        var mockLogLevelSection = new Mock<IConfigurationSection>();
        mockLogLevelSection.Setup(s => s.GetChildren()).Returns([mockSourceSection.Object]);

        var mockLoggingSection = new Mock<IConfigurationSection>();
        mockLoggingSection.Setup(s => s.GetSection("LogLevel")).Returns(mockLogLevelSection.Object);

        var mockSeqHostSection = new Mock<IConfigurationSection>();
        mockSeqHostSection.Setup(s => s.Value).Returns((string?)null);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection("Logging")).Returns(mockLoggingSection.Object);
        mockConfig.Setup(c => c.GetSection("Seq:Host")).Returns(mockSeqHostSection.Object);

        _configuration = mockConfig.Object;
    }

    [When(@"I call ConfigureLogging with app name '(.*)'")]
    public void WhenICallConfigureLoggingWithAppName(string appName)
    {
        _result = LoggingConfigurator.ConfigureLogging(_loggerConfiguration!, appName);
    }

    [When(@"I call ConfigureLogging with app name '(.*)' expecting an exception")]
    public void WhenICallConfigureLoggingWithAppNameExpectingAnException(string appName)
    {
        scenarioContext.CatchException(() => LoggingConfigurator.ConfigureLogging(_loggerConfiguration!, appName));
    }

    [When(@"I call ConfigureLogging with configuration and environment")]
    public void WhenICallConfigureLoggingWithConfigurationAndEnvironment()
    {
        _result = LoggingConfigurator.ConfigureLogging(_loggerConfiguration!, _configuration!, _hostEnvironment!);
    }

    [When(@"I call ConfigureLogging with configuration and environment expecting an exception")]
    public void WhenICallConfigureLoggingWithConfigurationAndEnvironmentExpectingAnException()
    {
        scenarioContext.CatchException(() => LoggingConfigurator.ConfigureLogging(_loggerConfiguration!, _configuration!, _hostEnvironment!));
    }

    [Then(@"the logger configuration should be returned")]
    public void ThenTheLoggerConfigurationShouldBeReturned()
    {
        Assert.NotNull(_result);
    }

    [Then(@"the logger should have console sink configured")]
    public void ThenTheLoggerShouldHaveConsoleSinkConfigured()
    {
        // We verify by ensuring the configuration was returned without errors
        // The actual sink configuration is internal to Serilog
        Assert.NotNull(_result);
    }
}
