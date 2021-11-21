namespace Asm.Domain
{
	public interface IIdentifiable<T>
	{
		T Id { get; }
	}
}