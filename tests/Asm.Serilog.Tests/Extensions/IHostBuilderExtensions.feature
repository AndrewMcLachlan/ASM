Feature: IHostBuilderExtensions
    Extensions for IHostBuilder to use custom Serilog

@Unit
Scenario: UseCustomSerilog configures Serilog on host builder
    Given I have a HostBuilder
    When I call UseCustomSerilog
    Then the host builder should be returned
