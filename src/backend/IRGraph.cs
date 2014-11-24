using System;
using System.Collections.Generic; // For SortedDictionary

// Type of an IRStream; we can change this if we need something more sophisticated later
using IRStream = System.Collections.Generic.List<IRTuple>;

public class IRGraph
{
  // Store this as a sorted dictionary so it is easy to iterate; by definition, blocks[0] is the entry node
  SortedDictionary<int, IRBlock> blocks;

  // Construct a graph from a stream of tuples
  public IRGraph(IRStream tuples)
  {
    this.blocks = new SortedDictionary<int, IRBlock>();

    if(tuples.Count < 1)
      return;

    int currentIndex = 0; // The next block index in the graph
    List<int> leaderIndices = new List<int>(); // Ordered list of indices where each basic block's leading tuple is found

    // By definition, the very first tuple is a leader, and belongs to the first block    
    leaderIndices.Add(0);
    this.blocks[currentIndex] = new IRBlock(currentIndex);
    this.blocks[currentIndex].AppendStatement(tuples[0]);

    int j = 1;
    while(j < tuples.Count)
    {
      IRTuple tuple = tuples[j];

      if(IsTerminator(tuple))
      {
        this.blocks[currentIndex].AppendStatement(tuple);
        currentIndex++;

        // If we find a statement that closes current block and there is not one obviously opened by the following tuple, create a new block
        if(j != tuples.Count-1 && !IsLeader(tuples[j+1]))
        {
          this.blocks[currentIndex] = new IRBlock(currentIndex);
          leaderIndices.Add(j+1);
        }

      }

      else
      {
        if(IsLeader(tuple))
        {
          // If we find a statement that starts a block, create a new block
          currentIndex++;
          this.blocks[currentIndex] = new IRBlock(currentIndex);
          leaderIndices.Add(j);
        }
        this.blocks[currentIndex].AppendStatement(tuple);
      }
      j++;
    }

/*
    Console.WriteLine("Block Leading Statements:");
    for (int i = 0; i < leaderIndices.Count; i++)
    {
      int index = leaderIndices[i];
      Console.Write("Statement " + index + "\t");
      tuples[index].Print();
    }
    */
  }

  // Return whether a tuple is of the type that may start a block
  private bool IsLeader(IRTuple tuple)
  {
    return (tuple.getOp() == IrOp.FUNC || tuple.getOp() == IrOp.LABEL);
  }

  // Return whether a tuple is of the type that may terminate a block
  private bool IsTerminator(IRTuple tuple)
  {
    return (tuple.getOp() == IrOp.JMP || tuple.getOp() == IrOp.JMPF || tuple.getOp() == IrOp.RET);
  }

  public void Print()
  {
    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
    {
      Console.WriteLine("Block " + pair.Value.GetIndex() + ":");
      pair.Value.PrintStatements();
      pair.Value.PrintSuccessors();
      Console.WriteLine();
    }
  }
}
