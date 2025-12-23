Feature: AzureOAuthOptions

@Unit
Scenario: AzureOAuthOptions Authority includes TenantId
    Given I have AzureOAuthOptions with domain 'https://login.microsoftonline.com' and tenant id '12345678-1234-1234-1234-123456789abc'
    Then the Authority should be 'https://login.microsoftonline.com/12345678-1234-1234-1234-123456789abc/v2.0'

@Unit
Scenario: AzureOAuthOptions inherits from OAuthOptions
    Given I have AzureOAuthOptions with domain 'https://login.microsoftonline.com' and audience 'api://myapp'
    Then the Audience should be 'api://myapp'
