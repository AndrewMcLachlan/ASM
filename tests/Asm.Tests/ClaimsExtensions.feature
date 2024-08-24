Feature: ClaimsPrincipalExtensions
  In order to retrieve claim values from a ClaimsPrincipal
  As a developer
  I want to have unit tests for the ClaimsPrincipal extension methods

Scenario: Get claim value as Guid
    Given I have a ClaimsPrincipal with a claim of type "claimType" and value "d3b07384-d9a1-4d3b-8a3e-8b0a1e4e1e1e"
    When I get the claim value as Guid
    Then the GUID result should be "d3b07384-d9a1-4d3b-8a3e-8b0a1e4e1e1e"

Scenario: Get claim value as int
    Given I have a ClaimsPrincipal with a claim of type "claimType" and value "123"
    When I get the claim value as int
    Then the integer result should be 123

Scenario: Get claim value as string
    Given I have a ClaimsPrincipal with a claim of type "claimType" and value "testValue"
    When I get the claim value as string
    Then the string result should be "testValue"

Scenario: Get claim value when claim does not exist
    Given I have a ClaimsPrincipal with no claims of type "nonExistentClaimType"
    When I get the claim value as string
    Then the result should be null
