using MediatR;

namespace Asm.Cqrs.Commands;

public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
{
    new Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken) => Handle(request, cancellationToken);
}


public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest> where TRequest : ICommand
{
    new Task Handle(TRequest request, CancellationToken cancellationToken);

    Task IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken) => Handle(request, cancellationToken);
}
