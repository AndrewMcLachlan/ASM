Feature: IEndpointConventionBuilderExtensions
    Extension methods for IEndpointConventionBuilder

@Unit
Scenario: WithNames sets display name and name
    Given I have a route handler builder
    When I call WithNames with 'Get Users'
    Then the display name should be set
