Feature: IQueryableExtensions
  In order to use IQueryable extension methods
  As a developer
  I want to have unit tests for the IQueryable extension methods

@Unit
Scenario: Paginate data using Page method
    Given I have a data source with the following items
        | Id | Name  |
        | 1  | Item1 |
        | 2  | Item2 |
        | 3  | Item3 |
        | 4  | Item4 |
        | 5  | Item5 |
    When I retrieve page 2 with a page size of 2
    Then the result should contain the following items
        | Id | Name  |
        | 3  | Item3 |
        | 4  | Item4 |

@Unit
Scenario: Filter data using WhereAny method
    Given I have a data source with the following items
        | Id | Name  |
        | 1  | Item1 |
        | 2  | Item2 |
        | 3  | Item3 |
        | 4  | Item4 |
        | 5  | Item5 |
    When I filter the data with predicates "Id == 1" or "Name == 'Item3'"
    Then the result should contain the following items
        | Id | Name  |
        | 1  | Item1 |
        | 3  | Item3 |
