using ThreadPool;
using System.Diagnostics;

namespace Tests;

[TestClass]
public class TestThreadPool
{
    [TestMethod]
    public void TestThreadPoolBasic()
    {
        ThreadPool.ThreadPool pool = new ThreadPool.ThreadPool(8);
        int result = 0;
        int expected = 0;
        for (int i = 0; i < 3000; i++) {
            int a = i;
            MyTask<int> task = (MyTask<int>)new MyTask<int>(() => {return a;}, pool).ContinueWith<int>(x => result += x * x);
            pool.EnqueueTask(task);
            expected += a * a;
        }

        Thread.Sleep(2000);
        Assert.AreEqual(expected, result);
        pool.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolException()
    {
        ThreadPool.ThreadPool pool = new ThreadPool.ThreadPool(8);
        int a = 0;
        MyTask<int> task = new MyTask<int>(() => 42 / a, pool);
        pool.EnqueueTask(task);
        Thread.Sleep(2000);
        Assert.AreEqual("Attempted to divide by zero.", task.exception.InnerException.Message);
        pool.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolContinueWith()
    {
        ThreadPool.ThreadPool pool = new ThreadPool.ThreadPool(2);
        int a = 0;
        MyTask<int> task1 = new MyTask<int>(() => 2, pool);
        MyTask<int> task2 = (MyTask<int>)new MyTask<int>(() => 2, pool).ContinueWith<int>(x => x * 2);
        MyTask<int> task3 = (MyTask<int>)new MyTask<int>(() => 2, pool).ContinueWith<int>(x => x * 2).ContinueWith<int>(x => x * 2);
        MyTask<int> task4 = (MyTask<int>)new MyTask<int>(() => 2, pool).ContinueWith<int>(x => x * 2).ContinueWith<int>(x => x * 2).ContinueWith<int>(x => x * 2);
        pool.EnqueueTask(task4);
        pool.EnqueueTask(task3);
        pool.EnqueueTask(task2);
        pool.EnqueueTask(task1);
        Thread.Sleep(1000);
        Assert.AreEqual(2, task1.Result);
        Assert.AreEqual(4, task2.Result);
        Assert.AreEqual(8, task3.Result);
        Assert.AreEqual(16, task4.Result);
        pool.Dispose();
    }

    [TestMethod]
    public void TestThreadCount()
    {
        int expected = 8;
        ThreadPool.ThreadPool pool = new ThreadPool.ThreadPool(expected);
        HashSet<int> startedThreads = new HashSet<int>();

        List<IMyTask<int>> tasks = new List<IMyTask<int>>();
        for (int i = 0; i < 100; i++)
        {
            MyTask<int> task = new MyTask<int>(() => {Thread.Sleep(12); return Thread.CurrentThread.ManagedThreadId;}, pool);
            tasks.Add(task);
            pool.EnqueueTask(task);
            Console.WriteLine(task.Result);
        }

        foreach (var task in tasks) {
            startedThreads.Add(task.Result);
        }
        pool.Dispose();

        Assert.AreEqual(expected, startedThreads.Count);
    }
}