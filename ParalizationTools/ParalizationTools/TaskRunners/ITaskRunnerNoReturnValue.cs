using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunners
{
    public interface ITaskRunner
    {
        void RunParallel();
        void AddTask(Task t);
    }

}
