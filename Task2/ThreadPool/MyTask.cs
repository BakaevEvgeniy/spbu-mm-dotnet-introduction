namespace ThreadPool;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> func_;
    private TResult result_;
    private ThreadPool pool_;
    public Exception? exception = null;
    public bool IsCompleted { get; private set; }
    public bool IsCompletedSuccessfully { get; private set; }
    public bool IsAborted { get; private set; }

    public void Abort()
    {
        IsCompleted = false;
        IsCompletedSuccessfully = false;
        IsAborted = true;
    }

    public MyTask(Func<TResult> func, ThreadPool pool)
    {
        func_ = func;
        pool_ = pool;
    }

    public TResult? Result
    {
        get
        {
            while (!IsCompleted)
            {
                if (IsCompleted)
                {
                    return result_;
                }
                else if (IsAborted)
                {
                    return default;
                }
                Thread.Sleep(10);
            }


            if (exception != null) throw new AggregateException(exception);

            return result_;
        }
    }

    public void Run()
    {
        if (IsCompleted) throw new InvalidOperationException();

        try
        {
            result_ = func_();
            IsCompletedSuccessfully = true;
            IsCompleted = true;
        }
        catch (Exception e)
        {
            exception = new AggregateException(e);
            IsCompletedSuccessfully = false;
            IsCompleted = true;
        }
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        pool_.EnqueueTask(this);
        var newTask = new MyTask<TNewResult>(() => continuation(Result), pool_);
        return newTask;
    }
}