using MediatR;

namespace Asm.Domain;
public interface IDomainEventHandler<TNotification> : INotificationHandler<TNotification> where TNotification : IDomainEvent
{
}
