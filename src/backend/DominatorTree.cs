using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTree
{
  private DominatorTreeNode startDominatorTree;
  private Dictionary<int, List<int>> dominateFrontiers;

  public DominatorTree(IRGraph cfg) {
    this.dominatorTree = this.BuildDominatorTree(cfg);

    this.ConstructDominanceFrontierMapping(cfg);
  }

  private DominatorTree BuildDominatorTree(IRGraph graph) {
    // Using Aho & Ullman - The thoery of parsing... Algorithm 11.5

  }

  public static SortedSet<IRBlock> FindReachableBlocks(IRGraph graph, int ignoreBlockIndex) {
    IRBlock head = graph.GetGraphHead();
    SortedSet<IRBlock> reachable = new SortedSet<IRBlock>();

    reachable.Add(heaed);

    return this.FindReachableBlocks(reachable, head, ignoreBlockIndex);
  }

  private static SortedSet<IRBlock> FindReachableBlocks(SortedSet<IRBlock> reachable, IRBlock block, int ignoreBlockIndex) {
    List<IRBlock> successors = block.GetSuccessors();
    foreach (IRBlock successor in successors) {
      if ((!reachable.Contains(successor)) && (successor.GetIndex() != ignoreBlockIndex)) {
        reachable.Add(successor);
        this.FindReachableBlocks(reachable, successor, ignoreBlockIndex);
      }
    }
    return reachable;
  }

  private void ConstructDominanceFrontierMapping(IRGraph graph) {
    // Using Ron Cytron et al Algorithm quadratic algorithm
  }

}