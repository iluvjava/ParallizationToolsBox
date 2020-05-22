using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskRunners;
using ThreadSafeDataStructures;

namespace TaskRunners
{

    /// <summary>
    ///     Queue Based Task Runner for tasks without return values. 
    /// </summary>
    public class QueueBaseTaskRunner : AbstractTaskRnner, ITaskRunner
    {

        ParallelCollection<Task> tasks_;

        public void QueueBasedTaskRunner(Queue<Task> tasksList)
        {
            tasks_ = new ParallelPool<Task>(tasksList);
        }

        public void AddTask(Task t)
        {
            tasks_.Put(t);
        }

        

        protected override Thread GetThread()
        {
            Thread t = new Thread(
                () => {
                    while (true)
                    {
                        Task task = null;
                        if (!tasks_.TryGet(out task)) return;
                        task.Start();
                    }

                }
            );
            return t;
        }
    }

    /// <summary>
    ///     T is the return type of the task. 
    /// </summary>
    /// <typeparam name="T">
    /// The return type of the tasks. 
    /// </typeparam>
    public class BagBasedTaskRunner<T> : AbstractTaskRnner, ITaskRunner<T>
    {
        int _threadsAllowed = Environment.ProcessorCount;
        ParallelPool<Task<T>> tasks_;
        public Queue<T> results_;

        public BagBasedTaskRunner(Queue<Task<T>> listOfTasks) : this(listOfTasks, new Queue<T>())
        { }

        public BagBasedTaskRunner(Queue<Task<T>> listOfTasks, Queue<T> ListOfResults)
        {
            tasks_ = new ParallelPool<Task<T>>(listOfTasks);
            results_ = ListOfResults;
        }

        override
        protected Thread GetThread()
        {
            Thread t = new Thread(
                    () => {
                        while (true)
                        {
                            Task<T> task = null;
                            if (!tasks_.TryGet(out task)) return;
                            task.Start();
                            AddResult(task.Result);
                        }

                    }
                );
            return t;
        }

      

        public void AddResult(T result)
        {
            lock (results_)
                results_.Enqueue(result);
        }

        public void AddTask(Task<T> t)
        {
            tasks_.Put(t);
        }

        public Queue<T> GetResult()
        {
            return results_;
        }
    }

}
