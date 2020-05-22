using TaskRunners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunners
{

   
   

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


}
