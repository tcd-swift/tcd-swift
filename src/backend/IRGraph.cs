using System;
using System.Collections.Generic; // For SortedDictionary

using IRStream = System.Collections.Generic.List<IRTuple>;
using Ident = System.String;

public class IRGraph
{
  // Store this as a sorted dictionary so it is easy to iterate; by definition, blocks[0] is the entry node
  SortedDictionary<int, IRBlock> blocks;

  // Construct a graph from a stream of tuples
  public IRGraph(IRStream tuples)
  {
    if(tuples.Count < 1)
      return;

    this.blocks = new SortedDictionary<int, IRBlock>();

    SortedDictionary<int, int> firsts; // Map from block index to line index of its first tuple in stream
    SortedDictionary<int, int> lasts; // Map from block index to line index of its last tuple in stream
 
    // Split the IR stream into blocks
    this.SplitStream(tuples, out firsts, out lasts);

    // Link successor blocks
    this.LinkSuccessors(firsts, lasts);
  
    // Compute initial liveness
    this.ComputeLiveness();
  }

  // 
  private void SplitStream(IRStream tuples, out SortedDictionary<int, int> firsts, out SortedDictionary<int, int> lasts)
  {
    firsts = new SortedDictionary<int, int>(); // Map from block index to line index of its first tuple in stream
    lasts = new SortedDictionary<int, int>(); // Map from block index to line index of its last tuple in stream

    int currentIndex = 1; // The next block index in the graph
    firsts[currentIndex] = 0; // By definition, the very first tuple is the first tuple in the first block
    this.blocks[currentIndex] = new IRBlock(currentIndex);
    this.blocks[currentIndex].AppendStatement(tuples[0]);

    int j = 1;
    while(j < tuples.Count)
    {
      IRTuple tuple = tuples[j];

      if(IsTerminator(tuple))
      {
        this.blocks[currentIndex].AppendStatement(tuple);
        lasts[currentIndex] = j;

        // If we find a statement that closes current block and there is not one obviously opened by the following tuple, create a new block
        if(j != tuples.Count-1 && !IsLeader(tuples[j+1]))
        {
          currentIndex++;
          this.blocks[currentIndex] = new IRBlock(currentIndex);
          firsts[currentIndex] = j + 1;
        }
      }

      else
      {
        if(IsLeader(tuple))
        {
          lasts[currentIndex] = j-1;
          // If we find a statement that starts a block, create a new block
          currentIndex++;
          this.blocks[currentIndex] = new IRBlock(currentIndex);
          firsts[currentIndex] = j;
        }
        this.blocks[currentIndex].AppendStatement(tuple);
      }
      j++;
      if(j == tuples.Count)
        lasts[currentIndex] = j-1;
    }
  }

  // Establish pointers between each block in the graph and its successor blocks
  private void LinkSuccessors(SortedDictionary<int, int> firsts, SortedDictionary<int, int> lasts)
  {
        foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
    {
      IRBlock block = pair.Value;
      IRTuple tup = block.GetLast(); // Get last statement in block
      if(tup.getOp() == IrOp.JMP)
      {
        foreach (KeyValuePair<int, IRBlock> pair0 in this.blocks)
        {
          IRBlock block0 = pair0.Value;
          IRTuple tup0 = block0.GetFirst();
          if(tup0.getOp() == IrOp.LABEL && tup0.getDest() == tup.getDest())
          {
            block.AddSuccessor(block0);
          }
        }
      }
      else if(tup.getOp() == IrOp.JMPF)
      {
        foreach (KeyValuePair<int, IRBlock> pair0 in this.blocks)
        {
          IRBlock block0 = pair0.Value;
          IRTuple tup0 = block0.GetFirst();
          if(tup0.getOp() == IrOp.LABEL && tup0.getDest() == ((IRTupleOneOpIdent)tup).getDest())
          {
            block.AddSuccessor(block0);
          }
          else if(firsts[block0.GetIndex()] == lasts[block.GetIndex()]+1)
          {
            block.AddSuccessor(block0);
          } 
        }

      }
      else
      {
        foreach (KeyValuePair<int, IRBlock> pair0 in this.blocks)
        {
          IRBlock block0 = pair0.Value;
          if(firsts[block0.GetIndex()] == lasts[block.GetIndex()]+1)
          {
            block.AddSuccessor(block0);
          } 
        }
      }
    }
  }

  // Compute block-based and statement-based liveness for this graph
  public void ComputeLiveness()
  {
    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
      pair.Value.ComputeLiveuseDef(); // Initialize events and anti-events for each block

    this.ComputeBlockLiveness(); // Iteratively determine block-based liveness

    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
      pair.Value.ComputeLiveouts(); // Determine statement-based liveness
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

  // Compute LiveIn and LiveOut for each block in this graph
  private void ComputeBlockLiveness()
  {
    bool converged = true;
    do{
      converged = true;
      foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
      {
        IRBlock block = pair.Value;
        HashSet<Ident> oldlivein = new HashSet<Ident>(block.GetLiveIn());
        HashSet<Ident> oldliveout = new HashSet<Ident>(block.GetLiveOut());
        block.UpdateLiveness();
        if(!block.GetLiveIn().SetEquals(oldlivein) || !block.GetLiveOut().SetEquals(oldliveout))
          converged = false;
      }
    } while(!converged);
  }

  public void Print()
  {
    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
    {
      IRBlock block = pair.Value;
      Console.WriteLine("Block " + block.GetIndex() + ":");
      block.PrintStatements();
      block.PrintSuccessors();
      block.PrintLiveuseDef();
      block.PrintLiveInOut();
      block.PrintLiveouts();
      Console.WriteLine();
    }
  }
}
