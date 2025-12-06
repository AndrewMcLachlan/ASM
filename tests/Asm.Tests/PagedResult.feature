Feature: PagedResult

@Unit
Scenario: Create PagedResult with results and total
    Given I have a list of items [1, 2, 3, 4, 5]
    When I create a PagedResult with total 100
    Then the PagedResult should have 5 results
    And the PagedResult total should be 100

@Unit
Scenario: Create PagedResult with empty results
    Given I have an empty list of items
    When I create a PagedResult with total 0
    Then the PagedResult should have 0 results
    And the PagedResult total should be 0
