# Asm.Cqrs.AspNetCore

Maps [Asm.Cqrs](https://github.com/AndrewMcLachlan/ASM/tree/main/src/Asm.Cqrs) commands and queries directly
to ASP.NET Core minimal API endpoints, removing the boilerplate handler lambda between the route and the
dispatcher.

## Features

- **Query endpoints**: `MapQuery` (GET) and `MapPagedQuery` (GET with an `X-Total-Count` header).
- **Command endpoints**: `MapCommand` (POST), `MapPutCommand`, `MapPatchCommand` and `MapDelete`.
- **Create endpoints**: `MapPostCreate` and `MapPutCreate`, returning `201 Created` with a `Location` header via a named route.
- **Binding control**: bind the request from route/query parameters, the request body, or a mix of both.
- **OpenAPI metadata**: endpoints declare their response types via `Produces`.
- `CommandQueryController` base class for apps still using MVC controllers.

## Installation

```
dotnet add package Asm.Cqrs.AspNetCore
```

## Usage

The mapping extension methods live in the `Asm.AspNetCore` namespace.

```csharp
using Asm.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQueryHandlers(typeof(GetOrder).Assembly);
builder.Services.AddCommandHandlers(typeof(CreateOrder).Assembly);

var app = builder.Build();

var orders = app.MapGroup("/orders");

orders.MapQuery<GetOrders, IEnumerable<Order>>("/");
orders.MapQuery<GetOrder, Order>("/{id}")
      .WithName("GetOrder");
orders.MapPostCreate<CreateOrder, Order>("/", routeName: "GetOrder", o => new { id = o.Id });
orders.MapPutCommand<UpdateOrder, Order>("/{id}");
orders.MapDelete<DeleteOrder>("/{id}");

app.Run();
```

Each method returns the `RouteHandlerBuilder`, so the endpoint can be customised further
(`.WithName(...)`, `.RequireAuthorization(...)`, etc.).

### Request binding

Queries are always bound with `[AsParameters]`: each property of the query is bound from the route,
query string or headers using the standard minimal API rules.

```csharp
// GET /orders/42 -> GetOrder(42)
public record GetOrder(int Id) : IQuery<Order>;
```

Command mappings take a `CommandBinding` argument that controls how the command is built from the request:

- `CommandBinding.Parameters` (the default) — the command is bound with `[AsParameters]`. Combine with
  attributes on individual properties to mix sources, e.g. an ID from the route and a payload from the body:

  ```csharp
  // PUT /orders/42 with a JSON body -> UpdateOrder(42, <body>)
  public record UpdateOrder([FromRoute] int Id, [FromBody] OrderDetails Details) : ICommand<Order>;
  ```

- `CommandBinding.Body` — the whole command is deserialised from the JSON request body.

  ```csharp
  orders.MapCommand<CreateOrder, Order>("/", binding: CommandBinding.Body);
  ```

- `CommandBinding.None` — no binding attribute is applied; the framework's default inference applies
  (for a complex type this is usually the request body).

### Status codes

Command mappings return `200 OK` (or `204 No Content` for commands without a response, and `201 Created`
for the create mappings) by default. Pass a status code to override:

```csharp
orders.MapCommand<SubmitOrder, OrderReceipt>("/submit", StatusCodes.Status202Accepted);
```

### Error responses

Handlers are free to throw; this package does not translate exceptions. Pair it with
[Asm.AspNetCore](https://github.com/AndrewMcLachlan/ASM/tree/main/src/Asm.AspNetCore), whose exception
handler maps the `Asm` exception types (`NotFoundException`, `ExistsException`, `NotAuthorisedException`,
FluentValidation's `ValidationException`) to RFC 9457 problem-details responses, or register your own
`IExceptionHandler`.

### MVC controllers

For apps using MVC controllers, `CommandQueryController` provides a base class with `QueryDispatcher` and
`CommandDispatcher` properties. It is decorated with `[ApiController]` and `[Authorize]`.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
