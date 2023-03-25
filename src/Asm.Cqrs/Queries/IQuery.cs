using MediatR;

namespace Asm.Cqrs.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>
{

}
