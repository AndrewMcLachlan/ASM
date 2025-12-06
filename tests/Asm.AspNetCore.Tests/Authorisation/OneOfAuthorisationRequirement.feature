Feature: OneOfAuthorisationRequirement

@Unit
Scenario: Create OneOfAuthorisationRequirement with multiple requirements
    Given I have authorization requirements
        | RequirementName |
        | Requirement1    |
        | Requirement2    |
        | Requirement3    |
    When I create a OneOfAuthorisationRequirement
    Then the requirement should contain 3 requirements

@Unit
Scenario: Create OneOfAuthorisationRequirement with single requirement
    Given I have authorization requirements
        | RequirementName |
        | Requirement1    |
    When I create a OneOfAuthorisationRequirement
    Then the requirement should contain 1 requirements

@Unit
Scenario: Create OneOfAuthorisationRequirement with no requirements
    Given I have no authorization requirements
    When I create a OneOfAuthorisationRequirement
    Then the requirement should contain 0 requirements
