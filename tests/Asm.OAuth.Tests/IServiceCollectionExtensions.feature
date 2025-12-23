Feature: IServiceCollectionExtensions

@Unit
Scenario: AddOAuthOptions registers OAuthOptions
    Given I have a ServiceCollection
    When I call AddOAuthOptions with section path 'OAuth'
    Then an OptionsBuilder for OAuthOptions should be returned

@Unit
Scenario: AddAzureOAuthOptions registers AzureOAuthOptions
    Given I have a ServiceCollection
    When I call AddAzureOAuthOptions with section path 'AzureOAuth'
    Then an OptionsBuilder for AzureOAuthOptions should be returned
