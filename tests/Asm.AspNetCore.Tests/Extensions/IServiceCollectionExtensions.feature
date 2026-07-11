Feature: IServiceCollection Extensions
    Extension methods for registering ASP.NET Core services

@Unit
Scenario: AddAsmExceptionHandler registers the Asm exception handler
    Given I have a service collection with host environment
    When I call AddAsmExceptionHandler
    Then the Asm exception handler should be registered

@Unit
Scenario: AddPrincipalProvider registers HttpContextPrincipalProvider
    Given I have an empty service collection
    When I call AddPrincipalProvider
    Then IPrincipalProvider should be registered as HttpContextPrincipalProvider
    And IHttpContextAccessor should be registered

@Unit
Scenario: AddPrincipalProvider returns the service collection for chaining
    Given I have an empty service collection
    When I call AddPrincipalProvider
    Then the returned service collection should be the same instance
