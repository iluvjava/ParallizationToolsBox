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
        public Queue<IMHComputeNode<T>> children_ { get; set; }
        protected IMHComputeNode<T> parent_;
        protected int rank_;

        public MHComputeNode()
        {
            children_ = new Queue<IMHComputeNode<T>>();
        }

        public void AddChild(IMHComputeNode<T> child)
        {
            children_.Enqueue(child);
            child.RegisterParent(this);
            this.rank_++;
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
