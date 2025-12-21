Feature: HttpContext OpenTelemetry Processors
    Processors that enrich traces with user information from HttpContext

@Unit
Scenario: TraceProcessor adds user to activity when HttpContext has user
    Given I have an HttpContext with username 'traceuser'
    When the trace processor processes an activity
    Then the activity should have a User custom property with value 'traceuser'

@Unit
Scenario: TraceProcessor does nothing when HttpContext is null
    Given I have a null HttpContext for the processor
    When the trace processor processes an activity
    Then the activity should not have a User custom property
