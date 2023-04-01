Feature: Named Entity tests
Prove that named entity comparison works

Scenario Outline: Compare
	Given I have a named entity with name '<First Name>'
	And I have a second  named entity with name '<Second Name>'
	When I call first.CompareTo(second)
	Then the integer value <Result> is returned

Examples:
| First Name | Second Name | Result |
| Alice      | Bob         | -1     |
| Bob        | Alice       | 1      |
| Alice      | Alice       | 0      |
| Alice      | alice       | 0      |
| <NULL>     | <NULL>      | 0      |
|            |             | 0      |
|            | Alice       | -5     |
| <NULL>     | Alice       | -1     |
| Alice      |             | 5      |
| Alice      | <NULL>      | 1      |
| 1          | Alice       | -16    |
| Alice      | 1           | 16     |