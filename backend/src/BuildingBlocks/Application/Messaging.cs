namespace JewelryStore.BuildingBlocks.Application;

public interface ICommand<out TResult>;
public interface IQuery<out TResult>;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}

public sealed class MessageDispatcher(IServiceProvider services)
{
    public Task<TResult> Send<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        var handler = services.GetService(typeof(ICommandHandler<TCommand, TResult>))
            as ICommandHandler<TCommand, TResult>
            ?? throw new InvalidOperationException($"No handler is registered for {typeof(TCommand).Name}.");
        return handler.Handle(command, cancellationToken);
    }

    public Task<TResult> Query<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        var handler = services.GetService(typeof(IQueryHandler<TQuery, TResult>))
            as IQueryHandler<TQuery, TResult>
            ?? throw new InvalidOperationException($"No handler is registered for {typeof(TQuery).Name}.");
        return handler.Handle(query, cancellationToken);
    }
}
