Feature: StandardJwtBearerOptions

@Unit
Scenario: Create StandardJwtBearerOptions with OAuthOptions
    Given I have OAuthOptions with domain 'https://auth.example.com' and audience 'api://my-api' and clientId 'client123'
    When I create StandardJwtBearerOptions with the OAuthOptions
    Then the StandardJwtBearerOptions should have the correct OAuthOptions
    And the StandardJwtBearerOptions should have default JwtBearerEvents

@Unit
Scenario: StandardJwtBearerOptions can set custom JwtBearerEvents
    Given I have OAuthOptions with domain 'https://auth.example.com' and audience 'api://my-api' and clientId 'client123'
    And I have custom JwtBearerEvents
    When I create StandardJwtBearerOptions with the OAuthOptions and custom events
    Then the StandardJwtBearerOptions should have the custom JwtBearerEvents
