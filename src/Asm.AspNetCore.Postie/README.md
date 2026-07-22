# Asm.AspNetCore.Postie

Maps a paged query to an ASP.NET Core minimal API endpoint on top of
[Postie](https://www.nuget.org/packages/Postie.AspNetCore)'s mediator-agnostic
`IEndpointDispatcher`, keeping Asm's paging convention: the response body is the unwrapped page and
the total item count is returned in an `X-Total-Count` header, instead of a wrapping `PagedResult<T>`
object.

## Installation

```
dotnet add package Asm.AspNetCore.Postie
```

You also need a Postie adapter package for your mediator of choice (e.g. `Postie.AspNetCore.MediatR`)
registered alongside it.

## Usage

The mapping extension method lives in the `Asm.AspNetCore` namespace.

```csharp
using Asm.AspNetCore;
using Postie.AspNetCore;

var app = builder.Build();

var orders = app.MapGroup("/orders");

// GET /orders?pageNumber=2&pageSize=25 -> GetOrders(2, 25)
orders.MapPagedQuery<GetOrders, Order>("/");
```

By default the endpoint is a GET, and the query is bound with `[AsParameters]` from the route, query
string and headers:

```csharp
public record GetOrders(int PageNumber, int PageSize) : IQuery<PagedResult<Order>>;
```

Pass `QueryMethod.Post` (or `QueryMethod.Query`, for the HTTP `QUERY` method) to bind the query from
the request body instead:

```csharp
// POST /orders/search with a JSON body -> SearchOrders(<body>)
orders.MapPagedQuery<SearchOrders, Order>("/search", QueryMethod.Post);
```

Pass `binding` to override the idiomatic binding for the chosen `method`, e.g. to bind a GET from the
body or a POST from parameters.

The method returns the `RouteHandlerBuilder`, so the endpoint can be customised further
(`.WithName(...)`, `.RequireAuthorization(...)`, etc.).

### Response shape

The handler dispatches the query, expecting a `PagedResult<TResponse>` back from `IEndpointDispatcher`,
then:

- writes the `PagedResult<TResponse>.Total` count into an `X-Total-Count` response header, and
- returns `200 OK` with `PagedResult<TResponse>.Results` (not the wrapping `PagedResult<T>`) as the
  body.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
