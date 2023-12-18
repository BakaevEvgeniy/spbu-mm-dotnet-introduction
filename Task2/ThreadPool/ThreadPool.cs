using System.Threading;

namespace ThreadPool;

public class ThreadPool : IDisposable
{
    private readonly int maxThreads_;
    private readonly Queue<IBaseTask> tasksQueue_;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private CancellationToken cancellationToken;
    private bool disposed_;

    public ThreadPool(int maxThreads) 
    {
        cancellationToken = cancellationTokenSource.Token;
        maxThreads_ = maxThreads;
        tasksQueue_ = new Queue<IBaseTask>();

        StartWorkerThreads();
    }

    public void EnqueueTask(IBaseTask task)
    {
        Monitor.Enter(tasksQueue_);
        tasksQueue_.Enqueue(task);
        Monitor.PulseAll(tasksQueue_);
        Monitor.Exit(tasksQueue_);
    }

    public void Dispose()
    {
        if (!disposed_)
        {
            Monitor.Enter(tasksQueue_);
            for (int i = 0; i < tasksQueue_.Count; i++)
            {
                var task = tasksQueue_.Dequeue();
                task.Abort();
            }
            cancellationTokenSource.Cancel();
            disposed_ = true;
            // signal to threads inside TaskExecutionLoop to finish loop
            Monitor.PulseAll(tasksQueue_);
            Monitor.Exit(tasksQueue_);
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
                Monitor.Enter(tasksQueue_);
                IBaseTask task;
                
                if (tasksQueue_.TryDequeue(out task))
                {
                    Monitor.Exit(tasksQueue_);
                    task.Run();
                }
                else
                {
                    // wait until EnqueueTask pulse
                    Monitor.Wait(tasksQueue_);
                    Monitor.Exit(tasksQueue_);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

}
