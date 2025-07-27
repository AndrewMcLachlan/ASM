namespace Asm.Cqrs;

/// <summary>
/// An object that can be dispatched.
/// </summary>
public interface IDispatchable
{
}

/// <summary>
/// An object than can be dispatched.
/// </summary>
/// <typeparam name="TResponse">The response from the dispatched.</typeparam>
public interface IDispatchable<out TResponse> : IDispatchable
{
}
