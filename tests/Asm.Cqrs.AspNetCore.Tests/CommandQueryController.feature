Feature: CommandQueryController

@Unit
Scenario: CommandQueryController exposes QueryDispatcher
    Given I have a CommandQueryController instance
    Then the QueryDispatcher property should not be null

@Unit
Scenario: CommandQueryController exposes CommandDispatcher
    Given I have a CommandQueryController instance
    Then the CommandDispatcher property should not be null

@Unit
Scenario: ControllerName returns controller name without suffix
    Given I have a CommandQueryController instance
    When I call ControllerName with a controller type
    Then the result should be the controller name without "Controller" suffix
