Feature: String Extensions

@Unit
Scenario Outline: Append
	Given I have a string '<string>'
	And I have a separator ','
	When I Append '<append>' to the string
	Then the string value '<value>' is returned

	Examples:
	| string | append | value       |
	|        | Hello  | Hello       |
	| Hello  | World  | Hello,World |