Feature: Keyed Entity tests
Prove that keyed entity equality works

@Unit
Scenario Outline: Test equality with .Equals
	Given I have a keyed entity with ID <First ID>
	And I have a second  keyed entity with ID <Second ID>
	When I call first.Equals(second)
	Then the boolean value <Result> is returned

Examples:
| First ID | Second ID | Result |
| 1        | 1         | true   |
| 1        | 2         | false  |
| 1        | <NULL>    | false  |

@Unit
Scenario Outline: Test equality with ==
	Given I have a keyed entity with ID <First ID>
	And I have a second  keyed entity with ID <Second ID>
	When I check equality with ==
	Then the boolean value <Result> is returned

Examples:
| First ID | Second ID | Result |
| 1        | 1         | true   |
| 1        | 2         | false  |
| 2        | 1         | false  |
| 1        | <NULL>    | false  |
| <NULL>   | <NULL>    | true   |
| <NULL>   | 1         | false  |

@Unit
Scenario Outline: Test inequality with !=
	Given I have a keyed entity with ID <First ID>
	And I have a second  keyed entity with ID <Second ID>
	When I check inequality with !=
	Then the boolean value <Result> is returned

Examples:
| First ID | Second ID | Result |
| 1        | 1         | false  |
| 1        | 2         | true   |
| 2        | 1         | true   |
| 1        | <NULL>    | true   |
| <NULL>   | <NULL>    | false  |
| <NULL>   | 1         | true   |

@Unit
Scenario Outline: Test equality with the Eqality Comparer
	Given I have a keyed entity with ID <First ID>
	And I have a second  keyed entity with ID <Second ID>
	When I check equality with an equality comparer
	Then the boolean value <Result> is returned

Examples:
| First ID | Second ID | Result |
| 1        | 1         | true   |
| 1        | 2         | false  |
| 2        | 1         | false  |
| 1        | <NULL>    | false  |
| <NULL>   | <NULL>    | true   |
| <NULL>   | 1         | false  |