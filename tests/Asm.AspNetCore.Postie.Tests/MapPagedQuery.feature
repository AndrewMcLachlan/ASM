Feature: MapPagedQuery

@Unit
Scenario: GET paged query returns the page with a total count header
    Given a paged endpoint mapped with the default method
    And the dispatcher returns 2 items with 100 total
    When I GET the endpoint
    Then the response status should be 200
    And the response body should be the unwrapped items
    And the X-Total-Count header should be '100'
    And the dispatcher should have received the term 'y'

@Unit
Scenario: POST paged query binds the query from the body
    Given a paged endpoint mapped with method Post
    And the dispatcher returns 2 items with 40 total
    When I POST the criteria to the endpoint
    Then the response status should be 200
    And the X-Total-Count header should be '40'
    And the dispatcher should have received the term 'x'

@Unit
Scenario: QUERY-verb paged query binds the query from the body
    Given a paged endpoint mapped with method Query
    And the dispatcher returns 2 items with 7 total
    When I send a QUERY request with criteria to the endpoint
    Then the response status should be 200
    And the X-Total-Count header should be '7'
    And the dispatcher should have received the term 'x'

@Unit
Scenario: An undefined method value is rejected at map time
    Given a route builder
    When I map a paged endpoint with an undefined method value
    Then an ArgumentOutOfRangeException should be thrown for 'method'

@Unit
Scenario: GET paged query with an explicit Body binding binds the criteria from the body
    Given a paged endpoint mapped with method Get and binding Body
    And the dispatcher returns 2 items with 5 total
    When I send a GET request with criteria in the body to the endpoint
    Then the response status should be 200
    And the dispatcher should have received the term 'x'

@Unit
Scenario: A paged query with Default binding lets the framework infer the source
    Given a paged endpoint mapped with method Post and binding Default
    And the dispatcher returns 2 items with 5 total
    When I POST the criteria to the endpoint
    Then the response status should be 200
    And the dispatcher should have received the term 'x'

@Unit
Scenario: An undefined binding value is rejected at map time
    Given a route builder
    When I map a paged endpoint with an undefined binding value
    Then an ArgumentOutOfRangeException should be thrown for 'binding'
