Feature: Assembly Extensions

@Unit
Scenario: Get assembly version
    Given I have the Asm library assembly
    When I get the assembly version
    Then the version should exist
    And the version should contain a version number

@Unit
Scenario: Get assembly file version
    Given I have the Asm library assembly
    When I get the assembly file version
    Then the version string should not be empty
    And the version should contain a version number

@Unit
Scenario: Get assembly informational version
    Given I have the Asm library assembly
    When I get the assembly informational version
    Then the version string should not be empty

@Unit
Scenario: Get test assembly version
    Given I have the current assembly
    When I get the assembly version
    Then the version should exist

@Unit
Scenario: Get test assembly file version
    Given I have the current assembly
    When I get the assembly file version
    Then the version string should not be empty

@Unit
Scenario: Get test assembly informational version
    Given I have the current assembly
    When I get the assembly informational version
    Then the version string should not be empty
