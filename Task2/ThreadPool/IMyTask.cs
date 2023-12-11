namespace ThreadPool;

public interface IBaseTask
{
    bool IsCompleted { get; }
    bool IsCompletedSuccessfully { get; }
    bool IsAborted { get; }

    void Run();
    void Abort();
}

public interface IMyTask<TResult> : IBaseTask
{
    TResult Result { get; }
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunc);
}
