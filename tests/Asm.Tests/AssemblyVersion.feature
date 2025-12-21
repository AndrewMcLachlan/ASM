Feature: AssemblyVersion
    As a developer
    I want to access assembly version information
    So that I can display version info in my application

@Unit
Scenario: Access Version property
    When I access the AssemblyVersion.Version property
    Then the property access should not throw

@Unit
Scenario: Access FileVersion property
    When I access the AssemblyVersion.FileVersion property
    Then the property access should not throw

@Unit
Scenario: Access InformationalVersion property
    When I access the AssemblyVersion.InformationalVersion property
    Then the property access should not throw
