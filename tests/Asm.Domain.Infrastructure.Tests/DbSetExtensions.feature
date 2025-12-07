Feature: DbSet Extensions

@Unit
Scenario: FindAsync with null DbSet throws NullReferenceException
    Given I have a null DbSet
    When I call FindAsync on the null DbSet
    Then an exception of type 'System.NullReferenceException' is thrown
