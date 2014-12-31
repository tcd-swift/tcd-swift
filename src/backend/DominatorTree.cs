using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTree
{
  private IRGraph cfg;
  private DominatorTreeNode startDominatorTree;
  private Dictionary<int, DominatorTreeNode> blockToNodeMappings;
  private Dictionary<IRBlock, List<IRBlock>> dominanceFrontiers;

  public DominatorTree(IRGraph cfg) {
    this.cfg = cfg;
    this.blockToNodeMappings = new Dictionary<int, DominatorTreeNode>();

    this.startDominatorTree = this.BuildDominatorTree();
    this.dominanceFrontiers = this.ConstructDominanceFrontierMapping(cfg);
  }

  private DominatorTreeNode BuildDominatorTree() {
    // Using Aho & Ullman - The thoery of parsing and compilation Vol II ... Algorithm 11.5
    SortedSet<IRBlock> V = this.cfg.GetSetOfAllBlocks();
    SortedSet<IRBlock> VSansR = new SortedSet<IRBlock>();
    VSansR.UnionWith(V);
    VSansR.Remove(this.cfg.GetGraphHead()); // remove head from the set of all blocks
    
    // calculate which blocks dominates what list of blocks
    Dictionary<IRBlock, SortedSet<IRBlock>> dominates = new Dictionary<IRBlock, SortedSet<IRBlock>>();
    foreach  (IRBlock v in VSansR) {
      SortedSet<IRBlock> VSansv = new SortedSet<IRBlock>();
      VSansv.UnionWith(V);
      VSansv.Remove(v); // V - {v}
      SortedSet<IRBlock> S = FindReachableBlocks(this.cfg, v.GetIndex());

      VSansv.ExceptWith(S);

      dominates[v] = VSansv; // V - {v} - S
    }

    // use dominates sets to build the dominator tree


    return null;
  }

  public static SortedSet<IRBlock> FindReachableBlocks(IRGraph cfg, int ignoreBlockIndex) {
    IRBlock head = cfg.GetGraphHead();
    SortedSet<IRBlock> reachable = new SortedSet<IRBlock>();
    reachable.Add(head);

    // if you can't use head you should ge no where
    if (head.GetIndex() == ignoreBlockIndex) {
      return reachable; 
    }

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

  private Dictionary<IRBlock, List<IRBlock>> ConstructDominanceFrontierMapping(IRGraph graph) {
    // Using Ron Cytron et al Algorithm quadratic algorithm
    Dictionary<IRBlock, List<IRBlock>> mappings = new Dictionary<IRBlock, List<IRBlock>>();
    

    return mappings;
  }

  private SortedSet<IRBlock> ConstructDominanceFrontierMapping(IRBlock x) {
    // Using Ron Cytron et al Algorithm
    SortedSet<IRBlock> dfx = new SortedSet<IRBlock>();

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