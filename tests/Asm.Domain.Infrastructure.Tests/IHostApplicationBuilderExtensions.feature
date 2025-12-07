Feature: IHostApplicationBuilder Extensions

@Unit
Scenario: AddEntityFrameworkOpenTelemetry returns same builder instance
    Given I have a host application builder
    When I call AddEntityFrameworkOpenTelemetry
    Then the same builder instance is returned

@Unit
Scenario: AddEntityFrameworkOpenTelemetry registers OpenTelemetry services
    Given I have a host application builder with an empty service collection
    When I call AddEntityFrameworkOpenTelemetry
    Then OpenTelemetry services are registered

@Unit
Scenario: AddEntityFrameworkOpenTelemetry can be called multiple times
    Given I have a host application builder
    When I call AddEntityFrameworkOpenTelemetry multiple times
    Then no exception is thrown

@Unit
Scenario: AddEntityFrameworkOpenTelemetry with null builder throws exception
    Given I have a null host application builder
    When I call AddEntityFrameworkOpenTelemetry on the null builder
    Then an exception of type 'System.NullReferenceException' is thrown
