## Compute Tree In a Nutshell ##

* Compute Tree is a tree data sctructure that represents a recursive algorithm. 

* If we represents function calls as a node in a graph, then a recursive algorithm expands itself in a DFS manner and then it merges the results by combining solution from its sub-tasks(Combining results from Children of that nodes.)

* It consists of ComputeNode. 

    * A ComputeNode can branch out to other ComputeNode and merge. 

## ComputeNode ##

* A ComputeNode can branch, and merge their results, it contains all the parameters for computing a certain sub-problem of a recursive problem. 

## Run ComputeTree and ComputeNode in Parallel

* Branching out the elements in a DFS manner, multiple threads will take elements from the stack, spawn that ComputeNode and then distribute it back to the stack. 

* Merging the ComputeNode consists retriving the ComputeNode in topological order and keeps track of all the prerequisits for each compute node. 

* Threads retrieve a ComputeNode, wait of all its sub-tasks (Children) to complete and then merges the results from all its sub-tasks. 

### Efficiency of Parallel Computations:

* Maximum efficiency is reached if the Compute Tree is a complete Tree, which indicates a Divide and Conquer algorithms will have the best efficiency. 

* Mimum efficiency if the ComputeTree turns out to be degenerate. 

### Thread Safe DataStrcure. 

* Common Data Structure that is thread safe and can be used in/for the ComputeNode and ComputeTree. 


