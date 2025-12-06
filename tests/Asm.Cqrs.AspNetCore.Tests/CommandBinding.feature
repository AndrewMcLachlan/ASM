Feature: CommandBinding

@Unit
Scenario Outline: CommandBinding enum has correct values
    Then the CommandBinding enum value '<Name>' should equal <Value>

Examples:
    | Name       | Value |
    | None       | 0     |
    | Body       | 1     |
    | Parameters | 2     |

@Unit
Scenario: CommandBinding enum has exactly 3 values
    Then the CommandBinding enum should have 3 values
