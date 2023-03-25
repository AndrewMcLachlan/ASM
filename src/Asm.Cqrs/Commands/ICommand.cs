using MediatR;

namespace Asm.Cqrs.Commands;

public interface ICommand<out TResponse> : IRequest<TResponse>
{

}


public interface ICommand : IRequest
{

}
