Feature: DomainDbContext

@Integration
Scenario: SaveChangesAsync with no tracked entities does not publish
    Given I have a DomainDbContext with a mock publisher
    When I call SaveChangesAsync with acceptAllChangesOnSuccess true
    Then the result should be 0
    And Publish should not have been called

@Integration
Scenario: SaveChangesAsync with acceptAllChangesOnSuccess false does not publish when no entities
    Given I have a DomainDbContext with a mock publisher
    When I call SaveChangesAsync with acceptAllChangesOnSuccess false
    Then the result should be 0
    And Publish should not have been called

@Integration
Scenario: SaveChangesAsync with default cancellation token does not publish when no events
    Given I have a DomainDbContext with a mock publisher
    When I call SaveChangesAsync with default cancellation token
    Then the result should be 0
    And Publish should not have been called

@Integration
Scenario: SaveChanges with no entities returns zero and does not publish
    Given I have a DomainDbContext with a mock publisher
    When I call SaveChanges on the DomainDbContext
    Then the result should be 0
    And Publish should not have been called
