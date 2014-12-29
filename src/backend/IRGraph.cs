using System;
using System.Collections.Generic;
using TCDSwift;

using Ident = System.String;

public class IRGraph
{
  private static int BLOCK_INDEX_INITIAL = 1; // Index of first block in graph
  private SortedDictionary<int, IRBlock> blocks; // Mapping of block index number to block

  // Construct a graph from a stream of tuples
  public IRGraph(List<IRTuple> tuples)
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
    List<string> livein;
    List<List<string>> liveouts;
    this.ComputeLiveness(out livein, out liveouts);
  }

  // Split an IR stream into this graph; firsts and lasts are maps of indices of the first and last index in the stream of each block
  private void SplitStream(List<IRTuple> tuples, out SortedDictionary<int, int> firsts, out SortedDictionary<int, int> lasts)
  {
    firsts = new SortedDictionary<int, int>();
    lasts = new SortedDictionary<int, int>();

    int currentIndex = BLOCK_INDEX_INITIAL; // The next block index in the graph
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
  public void ComputeLiveness(out List<string> livein, out List<List<string>> liveouts)
  {
    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
      pair.Value.ComputeLiveuseDef(); // Initialize events and anti-events for each block

    this.ComputeBlockLiveness(); // Iteratively determine block-based liveness

    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
      pair.Value.ComputeLiveouts(); // Determine statement-based liveness

    livein = new List<string>(this.blocks[BLOCK_INDEX_INITIAL].GetLiveIn());
    liveouts = new List<List<string>>();
    foreach (KeyValuePair<int, IRBlock> pair in this.blocks)
      liveouts.AddRange(pair.Value.GetLiveOuts());
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
