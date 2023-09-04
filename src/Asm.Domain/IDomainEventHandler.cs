using MediatR;

namespace Asm.Domain;
public interface IDomainEventHandler<TRequest> : IRequestHandler<TRequest> where TRequest : IDomainEvent
{
}
