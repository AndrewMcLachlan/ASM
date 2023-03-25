namespace Asm.Domain.Infrastructure;

public abstract class DbContextQueryHandler<TRequest, TResponse> : IQueryHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
{
    protected IReadOnlyDbContext Context { get; private set; }
    protected DbContextQueryHandler(IReadOnlyDbContext context)
    {
        Context = context;
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
