using MediatR;

namespace Asm.Cqrs.Queries;

/// <summary>
/// Represents a query that returns a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{

}
