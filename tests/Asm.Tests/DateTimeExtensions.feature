Feature: DateTime Extensions

@Unit
Scenario: Get First Day of Week
	This scenario depends on the locale in which the test is run
	Given I have date '2019-04-04'
	When I call FirstDayOfWeek
	Then the date '2019-03-31' is returned

@Unit
Scenario Outline: Get First Day of Week for a specific locale
	Given I have date '2019-04-04'
	And my locale is '<Locale>'
	When I call FirstDayOfWeek
	Then the date '<Result>' is returned

	Examples:
	| Locale | Result     |
	| en-US	 | 2019-03-31 |
	| en-GB	 | 2019-04-01 |

@Unit
Scenario: Get Last Day of Week
	This scenario depends on the locale in which the test is run
	Given I have date '2019-04-04'
	When I call LastDayOfWeek
	Then the date '2019-04-06' is returned

@Unit
Scenario Outline: Get Last Day of Week for a specific locale
	Given I have date '2019-04-04'
	And my locale is '<Locale>'
	When I call LastDayOfWeek
	Then the date '<Result>' is returned

	Examples:
	| Locale | Result     |
	| en-US	 | 2019-04-06 |
	| en-GB	 | 2019-04-07 |