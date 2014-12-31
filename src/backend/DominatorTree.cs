using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTree
{
  private DominatorTreeNode startDominatorTree;
  private Dictionary<int, DominatorTreeNode> blockToNodeMappings;
  private Dictionary<IRBlock, List<IRBlock>> dominateFrontiers;

  public DominatorTree(IRGraph cfg) {
    this.startDominatorTree = this.BuildDominatorTree(cfg);

    this.ConstructDominanceFrontierMapping(cfg);
  }

  private DominatorTreeNode BuildDominatorTree(IRGraph graph) {
    // Using Aho & Ullman - The thoery of parsing and compilation Vol II ... Algorithm 11.5
    SortedSet<IRBlock> V = graph.GetSetOfAllBlocks();
    SortedSet<IRGraph> VSansR = V.MemberwiseClone().Remove(graph.GetGraphHead()); // remove head from the set of all blocks
    
    // calculate which blocks dominates what list of blocks
    Dictionary<IRBlock, SortedSet<IRBlock>> dominates = Dictionary<IRBlock, SortedSet<IRBlock>>();
    foreach  (IRBlock v in VSansR) {
      SortedSet<IRBlock> temp = V.MemberwiseClone().Remove(v); // V - {v}
      SortedSet<IRBlock> S = FindReachableBlocks(graph, v.GetIndex());

      dominates[v] = temp.ExceptWith(S); // V - {v} - S
    }

    // use dominates sets to build the dominator tree


    return null;
  }

  public static SortedSet<IRBlock> FindReachableBlocks(IRGraph graph, int ignoreBlockIndex) {
    IRBlock head = graph.GetGraphHead();
    SortedSet<IRBlock> reachable = new SortedSet<IRBlock>();

    reachable.Add(head);

    return FindReachableBlocks(reachable, head, ignoreBlockIndex);
  }

  private static SortedSet<IRBlock> FindReachableBlocks(SortedSet<IRBlock> reachable, IRBlock block, int ignoreBlockIndex) {
    List<IRBlock> successors = block.GetSuccessors();
    foreach (IRBlock successor in successors) {
      if ((!reachable.Contains(successor)) && (successor.GetIndex() != ignoreBlockIndex)) {
        reachable.Add(successor);
        FindReachableBlocks(reachable, successor, ignoreBlockIndex);
      }
    }
    return reachable;
  }

  private void ConstructDominanceFrontierMapping(IRGraph graph) {
    // Using Ron Cytron et al Algorithm quadratic algorithm

  }

  private SortedSet<IRBlock> ConstructDominanceFrontierMapping(IRBlock x) {
    // Using Ron Cytron et al Algorithm
    SortedSet<IRBlock> dfx = SortedSet<IRBlock>();

    // DF(X)local
    // foreach (IRBlock y in x.GetSuccessors()) {
    //   if(idom(y) != x) {
    //     dfx.Add(y);
    //   }
    // }

    // // DF(X)up
    // DominatorTreeNode xt = this.blockToNodeMappings[x.GetIndex()];
    // foreach (DominatorTreeNode z in xt.GetDescendants()) {
    //   foreach (IRBlock y in df(z)) {
    //     if(idom(y) != x) {
    //       dfx.Add(y);
    //     }
    //   }
    // }

    return dfx;
  }

}