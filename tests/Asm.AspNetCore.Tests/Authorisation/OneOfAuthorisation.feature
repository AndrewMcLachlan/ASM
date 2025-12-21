Feature: One Of Authorisation
    Authorisation requirement that succeeds if any sub-requirement passes

@Unit
Scenario: OneOfAuthorisationRequirement stores requirements
    When I create a OneOfAuthorisationRequirement with 2 sub-requirements
    Then the requirement should have 2 sub-requirements
