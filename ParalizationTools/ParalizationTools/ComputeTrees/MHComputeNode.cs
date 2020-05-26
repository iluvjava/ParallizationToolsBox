using System;
using System.Collections.Generic;
using System.Text;

namespace ParalizationTools.ComputeTrees
{
    /// <summary>
    ///     A partially completed compute node, with return type T. 
    /// </summary>
    /// <typeparam name="T">
    ///     The return type of the compuet node. 
    /// </typeparam>
    public abstract class MHComputeNode<T> : IMHComputeNode<T>
    {
        private Queue<IMHComputeNode<T>> children_ { get; set; }
        private IMHComputeNode<T> parent_;

        public MHComputeNode()
        {
            children_ = new Queue<IMHComputeNode<T>>();
        }

        public void AddChild(IMHComputeNode<T> child)
        {
            children_.Enqueue(child);
            child.RegisterParent(this);
        }

        public abstract void Branch();
       

        public abstract void Merge();

        public void RegisterParent(IMHComputeNode<T> parent)
        {
            parent_ = parent;
        }

        public Queue<IMHComputeNode<T>> GetChildren()
        {
            return children_;
        }

        public IMHComputeNode<T> GetParent()
        {
            return parent_;
        }

        public abstract T GetResult();
    }
}
