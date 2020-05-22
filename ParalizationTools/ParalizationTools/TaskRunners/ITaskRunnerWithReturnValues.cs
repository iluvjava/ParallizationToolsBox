using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunners
{
    
    /// <summary>
    ///     A task runner with return values. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITaskRunner<T>
    {
        /// <summary>
        ///     Run all the tasks in parallel. 
        ///     And stored the results, execution will be out of order. 
        /// </summary>
        void RunParallel();
        void AddResult(T result);
        void AddTask(Task<T> t);

        Queue<T> GetResult();
    }

}
