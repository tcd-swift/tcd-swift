using System;
using System.Collections.Generic;
using System.Linq;
using TCDSwift;

public class DominatorTree
{
  private IRGraph cfg;
  private DominatorTreeNode rootTree;
  private Dictionary<IRBlock, DominatorTreeNode> blockToNodeMappings;
  private Dictionary<IRBlock, List<IRBlock>> dominanceFrontiers;

  public DominatorTree(IRGraph cfg) {
    this.cfg = cfg;
    this.blockToNodeMappings = new Dictionary<IRBlock, DominatorTreeNode>();

    this.rootTree = this.BuildDominatorTree();
    this.dominanceFrontiers = this.ConstructDominanceFrontierMapping(cfg);
  }

  public DominatorTreeNode GetRoot() {
    return this.rootTree;
  }

  private DominatorTreeNode BuildDominatorTree() {
    // Using Aho & Ullman - The thoery of parsing and compilation Vol II ... Algorithm 11.5
    IRBlock head = this.cfg.GetGraphHead();
    SortedSet<IRBlock> V = this.cfg.GetSetOfAllBlocks();
    SortedSet<IRBlock> VSansR = new SortedSet<IRBlock>();
    VSansR.UnionWith(V);
    VSansR.Remove(head); // remove head from the set of all blocks
    
    // calculate which blocks dominates what list of blocks
    Dictionary<IRBlock, SortedSet<IRBlock>> dominatesMapping = new Dictionary<IRBlock, SortedSet<IRBlock>>();
    dominatesMapping[head] = VSansR;
    foreach (IRBlock v in VSansR) {
      dominatesMapping[v] = CalculateDominatesSet(this.cfg, V, VSansR, v);
    }

    // use dominates sets to build the dominator tree
    SortedSet<IRBlock> placed = new SortedSet<IRBlock>();
    Dictionary<IRBlock, IRBlock> imediateDominator = new Dictionary<IRBlock, IRBlock>();

    CalculateImediateDominators(head, dominatesMapping, placed, imediateDominator);

    DominatorTreeNode headNode = BuildTreeFromImediateDominators(imediateDominator);

    return headNode;
  }

  private DominatorTreeNode BuildTreeFromImediateDominators(Dictionary<IRBlock, IRBlock> imediateDominator) {
    // create nodes
    foreach (IRBlock block in this.cfg.GetSetOfAllBlocks()) {
      this.blockToNodeMappings[block] = new DominatorTreeNode(block);
    }

    // link nodes
    foreach (KeyValuePair<IRBlock, IRBlock> pair in imediateDominator) {
      DominatorTreeNode ancestor = this.blockToNodeMappings[pair.Value];
      DominatorTreeNode descendant = this.blockToNodeMappings[pair.Key];

      ancestor.AddDescendant(descendant);
    }

    return this.blockToNodeMappings[this.cfg.GetGraphHead()];
  }

  private void CalculateImediateDominators(IRBlock block, Dictionary<IRBlock, SortedSet<IRBlock>> dominatesMapping, SortedSet<IRBlock> placed, Dictionary<IRBlock, IRBlock> imediateDominator) {
    placed.Add(block);

    foreach (IRBlock blocki in dominatesMapping[block]) {
      if(!placed.Contains(blocki)) {
        imediateDominator[blocki] = block;

        CalculateImediateDominators(blocki, dominatesMapping, placed, imediateDominator);
      }
    }
  }

  public static SortedSet<IRBlock> CalculateDominatesSet(IRGraph cfg, SortedSet<IRBlock> V, SortedSet<IRBlock> VSansR, IRBlock v) {
    SortedSet<IRBlock> VSansv = new SortedSet<IRBlock>();
    VSansv.UnionWith(V);
    VSansv.Remove(v); // V - {v}
    SortedSet<IRBlock> S = FindReachableBlocks(cfg, v.GetIndex());

    VSansv.ExceptWith(S);

    return VSansv; // V - {v} - S
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