Feature: OAuthOptions

@Unit
Scenario: OAuthOptions Authority returns Domain
    Given I have OAuthOptions with domain 'https://auth.example.com'
    Then the Authority should be 'https://auth.example.com'

@Unit
Scenario: OAuthOptions ValidateAudience defaults to false
    Given I have OAuthOptions with default ValidateAudience
    Then ValidateAudience should be false

@Unit
Scenario: OAuthOptions ValidateAudience can be set to true
    Given I have OAuthOptions with ValidateAudience set to true
    Then ValidateAudience should be true
