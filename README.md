# ASM Library
A collection of useful assemblies to assist in development

## Asm
- Extensions to .NET types, such as `Boolean`, `DateTime`, `String` etc.
- Exception types such as `NotFound`, `Exists`, `NotAuthorised` etc.

## Asm.AspNetCore
- Bootstrap a new web application, or web job.

### Modules
A way to breakdown your app into modules, that define their own endpoints and services. This is an alternative to microservices architecture.

## Asm.AspNetCore.Api
- Enables a `/metaz` endpoint for returning app metadata
- Standed setup for Open API document generation

## Asm.AspNetCore.Mvc
Helpers for MVC application using Razor.

## Asm.Cqrs
A basic CQRS implementation, built on Mediatr.

## Asm.Cqrs.AspNetCore
Binds API endpoints directly to CQRS commands and queries.

## Asm.Domain
Supports domain-driven-design.
Interfaces and attributes for
  - Entity
  - Aggregate Root
  - Repositories
  - Specifications
  - Unit of work
  - Domain Events

## Asm.Domain.Infrastructure
= Extensions that allow a `DBSet` to be injected as an `IQueryable`
- Base repository implementations
- Executing domain events on save

## Asm.Hosting
Boostraps a hosted application

## Asm.Net
Extensions for `IPAddress`

## Asm.OAuth
Standard configuration for apps that support OAuth.

## Asm.Serilog
Extensions that set up Serilog and connect to Seq.

## Asm.Testing
Helpers for building tests using SpecFlow.
