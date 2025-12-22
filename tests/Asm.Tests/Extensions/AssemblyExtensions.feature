Feature: AssemblyExtensions

@Unit
Scenario: Version returns assembly version
    Given I have the current assembly
    When I call the Version extension method
    Then the version should not be null

@Unit
Scenario: FileVersion returns file version info
    Given I have the current assembly
    When I call the FileVersion extension method
    Then the file version info should not be null

@Unit
Scenario: InformationalVersion returns informational version
    Given I have the current assembly
    When I call the InformationalVersion extension method
    Then the informational version should be a string or null
