Feature: DateTime Extensions

@Unit
Scenario: Get First Day of Week
	This scenario depends on the locale in which the test is run
	Given I have date '2019-04-04'
	When I call FirstDayOfWeek
	Then the a date '2019-03-31' will be returned

@Unit
Scenario Outline: Get First Day of Week for a specific locale
	Given I have date '2019-04-04'
	And my locale is '<Locale>'
	When I call FirstDayOfWeek
	Then the a date '<Result>' will be returned

	Examples:
	| Locale | Result     |
	| en-US	 | 2019-03-31 |
	| en-GB	 | 2019-04-01 |

@Unit
Scenario: Get Last Day of Week
	This scenario depends on the locale in which the test is run
	Given I have date '2019-04-04'
	When I call LastDayOfWeek
	Then the a date '2019-04-06' will be returned

@Unit
Scenario Outline: Get Last Day of Week for a specific locale
	Given I have date '2019-04-04'
	And my locale is '<Locale>'
	When I call LastDayOfWeek
	Then the a date '<Result>' will be returned

	Examples:
	| Locale | Result     |
	| en-US	 | 2019-04-06 |
	| en-GB	 | 2019-04-07 |