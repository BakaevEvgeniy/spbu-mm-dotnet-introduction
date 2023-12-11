using System.Threading;

namespace ThreadPool;

public class ThreadPool : IDisposable
{
    // private static ThreadPool _instance;
    private readonly int maxThreads_;
    private readonly Thread[] threads;
    private readonly Queue<IBaseTask> tasksQueue_;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private CancellationToken cancellationToken;
    private static Mutex mut_ = new Mutex();
    private bool disposed_;

    public ThreadPool(int maxThreads) 
    {
        cancellationToken = cancellationTokenSource.Token;
        maxThreads_ = maxThreads;
        threads = new Thread[maxThreads];
        tasksQueue_ = new Queue<IBaseTask>();

        StartWorkerThreads();
    }

    public void EnqueueTask(IBaseTask task)
    {
        mut_.WaitOne();
        tasksQueue_.Enqueue(task);
        mut_.ReleaseMutex();
    }

    public void Dispose()
    {
        if (!disposed_)
        {
            mut_.WaitOne();
            for (int i = 0; i < tasksQueue_.Count; i++)
            {
                var task = tasksQueue_.Dequeue();
                task.Abort();
            }
            cancellationTokenSource.Cancel();
            disposed_ = true;
            mut_.ReleaseMutex();
        }
    }

    private void StartWorkerThreads()
    {
        for (int i = 0; i < maxThreads_; i++)
        {
            Thread workerThread = new Thread(() => {TaskExecutionLoop(cancellationToken);});
            workerThread.Start();
        }
    }

    private void TaskExecutionLoop(CancellationToken cancellationToken)
    {
        while (!disposed_ && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                mut_.WaitOne();
                IBaseTask task;
                
                if (tasksQueue_.TryDequeue(out task))
                {
                    task.Run();
                }
                mut_.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

}
