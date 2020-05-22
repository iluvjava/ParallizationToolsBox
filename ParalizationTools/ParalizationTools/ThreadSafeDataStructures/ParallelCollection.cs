using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadSafeDataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ParallelCollection<E>
    {
        void Put(E item);
        bool TryGet(out E placeHolder);

        bool IsEmpty();
    }

}
