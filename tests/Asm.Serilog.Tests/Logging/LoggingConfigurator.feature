Feature: LoggingConfigurator
    Helper class to configure Serilog logging

@Unit
Scenario: ConfigureLogging with app name returns configured logger
    Given I have a LoggerConfiguration
    When I call ConfigureLogging with app name 'TestApp'
    Then the logger configuration should be returned
    And the logger should have console sink configured

@Unit
Scenario: ConfigureLogging with null configuration throws ArgumentNullException
    Given I have a null LoggerConfiguration
    When I call ConfigureLogging with app name 'TestApp' expecting an exception
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: ConfigureLogging with IConfiguration returns configured logger
    Given I have a LoggerConfiguration
    And I have a configuration with logging settings
    And I have a host environment for 'TestApp'
    When I call ConfigureLogging with configuration and environment
    Then the logger configuration should be returned

@Unit
Scenario: ConfigureLogging with null IConfiguration throws ArgumentNullException
    Given I have a LoggerConfiguration
    And I have a null configuration
    And I have a host environment for 'TestApp'
    When I call ConfigureLogging with configuration and environment expecting an exception
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: ConfigureLogging with null host environment throws ArgumentNullException
    Given I have a LoggerConfiguration
    And I have a configuration with logging settings
    And I have a null host environment
    When I call ConfigureLogging with configuration and environment expecting an exception
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario Outline: ConfigureLogging maps Microsoft log level names to Serilog levels
    Given I have a LoggerConfiguration
    And I have a configuration with default log level '<level>'
    And I have a host environment for 'TestApp'
    When I call ConfigureLogging with configuration and environment
    Then a logger created from the result has minimum level '<expected>'

Examples:
    | level       | expected    |
    | Trace       | Verbose     |
    | Debug       | Debug       |
    | Information | Information |
    | Warning     | Warning     |
    | Error       | Error       |
    | Critical    | Fatal       |
    | None        | Off         |
    | Verbose     | Verbose     |
    | Fatal       | Fatal       |

@Unit
Scenario: ConfigureLogging with a log level override of None does not throw
    Given I have a LoggerConfiguration
    And I have a configuration with default log level 'Information' and override for 'Microsoft.Hosting.Lifetime' set to 'None'
    And I have a host environment for 'TestApp'
    When I call ConfigureLogging with configuration and environment
    Then the logger configuration should be returned

@Unit
Scenario: ConfigureLogging ignores unrecognised log level names
    Given I have a LoggerConfiguration
    And I have a configuration with default log level 'NotALevel'
    And I have a host environment for 'TestApp'
    When I call ConfigureLogging with configuration and environment
    Then the logger configuration should be returned

