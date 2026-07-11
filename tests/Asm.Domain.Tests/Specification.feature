Feature: Specification Pattern
Tests for ISpecification and IQueryable.Specify extension

@Unit
Scenario: Apply specification to query filters correctly
    Given I have a collection of test entities with IDs [1, 2, 3, 4, 5]
    When I apply a specification that filters IDs greater than 2
    Then the result should contain entities with IDs [3, 4, 5]

@Unit
Scenario: Apply null specification returns original query
    Given I have a collection of test entities with IDs [1, 2, 3]
    When I apply a null specification
    Then the result should contain entities with IDs [1, 2, 3]

@Unit
Scenario: Apply specification using generic type parameter
    Given I have a collection of test entities with IDs [1, 2, 3, 4, 5]
    When I apply the GreaterThanTwoSpecification using type parameter
    Then the result should contain entities with IDs [3, 4, 5]

@Unit
Scenario: Compose specifications with AND
    Given I have a collection of test entities with IDs [1, 2, 3, 4, 5, 6]
    When I apply the greater-than-two AND less-than-five specification
    Then the result should contain entities with IDs [3, 4]

@Unit
Scenario: Compose specifications with OR
    Given I have a collection of test entities with IDs [1, 2, 3, 4, 5]
    When I apply the greater-than-two OR even specification
    Then the result should contain entities with IDs [2, 3, 4, 5]

@Unit
Scenario: Negate a specification with NOT
    Given I have a collection of test entities with IDs [1, 2, 3, 4]
    When I apply the NOT greater-than-two specification
    Then the result should contain entities with IDs [1, 2]
