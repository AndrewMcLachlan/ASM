Feature: Authentication
    JWT Bearer authentication extension methods

@Unit
Scenario: AddStandardJwtBearer registers JWT bearer authentication
    Given I have an authentication builder
    When I call AddStandardJwtBearer with default options
    Then the authentication builder should be returned
    And JwtBearerOptions should be configured

@Unit
Scenario: AddStandardJwtBearer with custom options applies configuration
    Given I have an authentication builder
    When I call AddStandardJwtBearer with custom options
    Then the authentication builder should be returned
