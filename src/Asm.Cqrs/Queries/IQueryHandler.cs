using MediatR;

namespace Asm.Cqrs.Queries;

public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
{
    new Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken) => Handle(request, cancellationToken);
}
