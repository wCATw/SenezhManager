using System;

namespace DiscordBot.Services.Structs;

public readonly struct DbResult<T>
{
    public T? Result { get; }
    public bool IsSuccess => Exception is null;
    public Exception? Exception { get; }

    public DbResult(T? result)
    {
        Result = result;
        Exception = null;
    }

    public DbResult(Exception exception)
    {
        Result = default;
        Exception = exception;
    }
}