Feature: StringBuilder Extensions

@Unit
Scenario: AppendFormatLine appends formatted text with newline
    Given I have a StringBuilder with content 'Hello'
    When I call AppendFormatLine with format ' {0}!' and arg 'World'
    Then the StringBuilder should contain 'Hello World!' followed by a newline

@Unit
Scenario: AppendFormatLine with format provider
    Given I have a StringBuilder with content 'Value: '
    When I call AppendFormatLine with InvariantCulture and format '{0:N2}' and arg 1234.5
    Then the StringBuilder should contain 'Value: 1,234.50' followed by a newline

@Unit
Scenario: AppendFormatLine with multiple arguments
    Given I have an empty StringBuilder
    When I call AppendFormatLine with format '{0} + {1} = {2}' and args [1, 2, 3]
    Then the StringBuilder should contain '1 + 2 = 3' followed by a newline
