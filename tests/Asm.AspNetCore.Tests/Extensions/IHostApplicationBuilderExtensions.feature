Feature: IHostApplicationBuilderExtensions
    Extension methods for IHostApplicationBuilder

@Unit
Scenario: AddStandardOpenTelemetry configures OpenTelemetry services
    Given I have a WebApplicationBuilder for OpenTelemetry
    When I call AddStandardOpenTelemetry
    Then the host application builder should be returned
    And OpenTelemetry services should be registered
