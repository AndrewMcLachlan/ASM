Feature: IEnumeratorExtensions

@Unit
Scenario: GetEnumerator converts IEnumerator to generic IEnumerator
    Given I have an ArrayList with values 1, 2, 3
    When I call GetEnumerator to convert to IEnumerator of int
    Then I should be able to enumerate and get values 1, 2, 3

@Unit
Scenario: GetEnumerator works with empty collection
    Given I have an empty ArrayList
    When I call GetEnumerator to convert to IEnumerator of int
    Then the enumeration should yield no results

@Unit
Scenario: GetEnumerator converts string enumerator
    Given I have an ArrayList with strings 'a', 'b', 'c'
    When I call GetEnumerator to convert to IEnumerator of string
    Then I should be able to enumerate and get strings 'a', 'b', 'c'
