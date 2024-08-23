Feature: Boolean Extensions

@Unit
Scenario Outline: Convert Boolean to a numeric value
	Given I have boolean that is '<Value>'
	When I call ToNumeric
	Then the integer value <Numeric> is returned

	Examples:
	| Value | Numeric |
	| true  | 1       |
	| false | 0       |

@Unit
Scenario Outline: Convert Boolean to a numeric string value
	Given I have boolean that is '<Value>'
	When I call ToNumericString
	Then the string value '<Numeric>' is returned

	Examples:
	| Value | Numeric |
	| true  | 1       |
	| false | 0       |