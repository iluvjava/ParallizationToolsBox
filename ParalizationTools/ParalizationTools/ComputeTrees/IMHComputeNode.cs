using System;
using System.Collections.Generic;
using System.Text;

namespace ParalizationTools.ComputeTrees
{
    /// <summary>
    ///     This is the most general type of compute node, branching and merging in parallel. 
    ///     * DFS branching by default. 
    /// </summary>
    /// <typeparam name="T">
    ///     The result type that is going to be returned from this computeNode. 
    /// </typeparam>
    public interface IMHComputeNode<T>
    {
        /// <summary>
        ///     Add a subtask to this compute node. 
        /// </summary>
        /// <param name="child"></param>
        void AddChild(IMHComputeNode<T> child);

        /// <summary>
        ///     Branching out your compute Node, please please PLEASE register all 
        ///     subtasks of this computenode using the addChild() method. 
        /// </summary>
        void Branch();

        /// <summary>
        ///     Merge the results for the compute node here, you 
        ///     can get the subtask you added using GetChildren() method 
        ///     * They will be in FIFO order. 
        /// </summary>
        void Merge();

        /// <summary>
        ///     Register a parent to this compute node with this method. 
        /// </summary>
        /// <param name="parent"></param>
        void RegisterParent(IMHComputeNode<T> parent);

        /// <summary>
        ///     Returns a queue having all the children in this compute node, 
        ///     * Intended to be used by the compute tree. 
        /// </summary>
        /// <returns></returns>
        Queue<IMHComputeNode<T>> GetChildren();

        /// <summary>
        ///     Get the parent of this compute node. 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        IMHComputeNode<T> GetParent();

        /// <summary>
        ///     Return the result for the compute node, client should implement this. 
        /// </summary>
        /// <returns></returns>
        T GetResult();


    }
}
