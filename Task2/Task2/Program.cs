using System;
using ThreadPool;

namespace Task2;

class Program
{
    static void Main(string[] args)
    {
        ThreadPool.ThreadPool pool = new ThreadPool.ThreadPool(4);
        // example
        MyTask<int> task1 = (MyTask<int>)new MyTask<int>(() => {return 42;}, pool).ContinueWith<int>(x => x * x).ContinueWith<int>(x => x - 1);
        pool.EnqueueTask(task1);
        Console.WriteLine(task1.Result);
        
        pool.Dispose();
    }
}