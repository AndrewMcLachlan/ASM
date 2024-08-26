Feature: Decimal Extensions

@Unit
Scenario Outline: To Rounded Currency String
	Given I have a decimal <decimal>
	And I want to round to <places> decimal places
	When I call ToRoundedCurrencyString
	Then the string value '<result>' is returned

	Examples:
	| decimal | places | result   |
	| 11.4321 | 2      | 11.43    |
	| 11.4351 | 2      | 11.44    |
	| 11.4321 | 0      | 11       |
	| 11.5    | 0      | 12       |
	| 11.4321 | 4      | 11.4321  |
	| 11.4321 | 5      | 11.43210 |

@Unit
Scenario Outline: To Rounded Currency String with fixed decimal places
	Given I have a decimal <decimal>
	When I call ToRoundedCurrencyString
	Then the string value '<result>' is returned

	Examples:
	| decimal | result |
	| 11.4321 | 11.43  |
	| 11.4351 | 11.44  |

@Unit
Scenario Outline: To Rounded Currency String out of range
		Given I have a decimal 4.321
	And I want to round to <places> decimal places
	When I call ToRoundedCurrencyString expecting an exception
	Then an exception of type 'System.ArgumentOutOfRangeException' is thrown

	Examples:
	| places |
	| -1     |
	| 29     |