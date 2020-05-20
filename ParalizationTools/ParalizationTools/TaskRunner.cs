using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunners
{
    /// <summary>
    ///     T is the return type of the task. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueBasedTaskRunner<T> : AbstractTaskRnner, ITaskRunner<T>
    {
        int _threadsAllowed = Environment.ProcessorCount;
        Queue<Task<T>> _tasks;
        public Queue<T> _results;

        public QueueBasedTaskRunner(Queue<Task<T>> listOfTasks) : this(listOfTasks, new Queue<T>())
        { }

        public QueueBasedTaskRunner(Queue<Task<T>> listOfTasks, Queue<T> ListOfResults)
        {
            _tasks = listOfTasks;
            _results = ListOfResults;
        }

        override
        protected Thread GetThread()
        {
            Thread t = new Thread(
                    () => {
                        while (true)
                        {
                            Task<T> task = GetTask();
                            if (task is null) break;
                            task.Start();
                            AddResult(task.Result);
                        }

                    }
                );
            return t;
        }

        public Task<T> GetTask()
        {
            lock (this)
            {
                if (_tasks.Count == 0)
                {
                    return null;
                }
                return _tasks.Dequeue();
            }
        }

        public void AddResult(T result)
        {
            lock (this)
                _results.Enqueue(result);
        }

        public void AddTask(Task<T> t)
        {
            lock (this)
            {
                Console.WriteLine("adding tasks...");
                _tasks.Enqueue(t);
            }
        }
    }

    /// <summary>
    ///     A task runner with return values. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITaskRunner<T>
    {
        void RunParallel();
        void AddResult(T result);
        Task<T> GetTask();
        void AddTask(Task<T> t);
    }

    public interface ITaskRunner
    {
        void RunParallel();
        Task GetTask();
        void AddTask(Task t);
    }

    public abstract class AbstractTaskRnner
    {
        int _threadsAllowed = Environment.ProcessorCount;

        public void RunParallel()
        {

            Thread[] threads = new Thread[_threadsAllowed];
            for (int I = 0; I < threads.Length; I++)
            {
                threads[I] = GetThread();
                threads[I].Start();
            }
            for (int I = 0; I < threads.Length; I++)
            {
                threads[I].Join();
            }

        }

        protected abstract Thread GetThread();

    }

    /// <summary>
    ///     Queue Based Task Runner for tasks without return values. 
    /// </summary>
    public class QueueBaseTaskRunner : AbstractTaskRnner, ITaskRunner
    {

        Queue<Task> _tasks;

        public void AddTask(Task t)
        {
            lock (this)
            {
                Console.WriteLine("adding tasks...");
                _tasks.Enqueue(t);
            }
        }

        public Task GetTask()
        {
            lock (this)
            {
                if (_tasks.Count == 0)
                {
                    return null;
                }
                return _tasks.Dequeue();
            }
        }

        protected override Thread GetThread()
        {
            Thread t = new Thread(
                () => {
                    while (true)
                    {
                        Task task = GetTask();
                        if (task is null) break;
                        task.Start();
                    }

                }
            );
            return t;
        }
    }


   
    
}
