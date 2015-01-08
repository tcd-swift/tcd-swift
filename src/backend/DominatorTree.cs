using System;
using System.Collections.Generic;
using System.Linq;
using TCDSwift;

public class DominatorTree
{
  private IRGraph cfg;
  private DominatorTreeNode rootNode;
  private Dictionary<IRBlock, IRBlock> imediateDominator;
  private Dictionary<IRBlock, DominatorTreeNode> blockToNodeMappings;
  private Dictionary<IRBlock, SortedSet<IRBlock>> dominanceFrontiers;

  public DominatorTree(IRGraph cfg)
  {
    this.cfg = cfg;
    this.imediateDominator = new Dictionary<IRBlock, IRBlock>();
    this.blockToNodeMappings = new Dictionary<IRBlock, DominatorTreeNode>();
    this.dominanceFrontiers = new Dictionary<IRBlock, SortedSet<IRBlock>>();

    // Build the tree and construct the dominance froniters
    this.BuildDominatorTree();
    this.ConstructDominanceFrontierMapping();
  }

  public DominatorTreeNode GetRoot()
  {
    return this.rootNode;
  }

  public DominatorTreeNode GetNode(IRBlock block)
  {
    return this.blockToNodeMappings[block];
  }

  public SortedSet<IRBlock> GetDominanceFrontier(IRBlock block)
  {
    return this.dominanceFrontiers[block];
  }

  private void BuildDominatorTree()
  {
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
    this.imediateDominator = new Dictionary<IRBlock, IRBlock>();

    CalculateImediateDominators(head, dominatesMapping, placed);

    this.rootNode = BuildTreeFromImediateDominators();
  }

  private DominatorTreeNode BuildTreeFromImediateDominators()
  {
    // create nodes
    foreach (IRBlock block in this.cfg.GetSetOfAllBlocks())
    {
      this.blockToNodeMappings[block] = new DominatorTreeNode(block);
    }

    // link nodes
    foreach (KeyValuePair<IRBlock, IRBlock> pair in this.imediateDominator)
    {
      DominatorTreeNode ancestor = this.blockToNodeMappings[pair.Value];
      DominatorTreeNode descendant = this.blockToNodeMappings[pair.Key];

      ancestor.AddDescendant(descendant);
    }

    return this.blockToNodeMappings[this.cfg.GetGraphHead()];
  }

  private void CalculateImediateDominators(IRBlock block, Dictionary<IRBlock, SortedSet<IRBlock>> dominatesMapping, SortedSet<IRBlock> placed)
  {
    placed.Add(block);

    foreach (IRBlock blocki in dominatesMapping[block])
    {
      if(!placed.Contains(blocki))
      {
        this.imediateDominator[blocki] = block;

        CalculateImediateDominators(blocki, dominatesMapping, placed);
      }
    }
  }

  public static SortedSet<IRBlock> CalculateDominatesSet(IRGraph cfg, SortedSet<IRBlock> V, SortedSet<IRBlock> VSansR, IRBlock v)
  {
    SortedSet<IRBlock> VSansv = new SortedSet<IRBlock>();
    VSansv.UnionWith(V);
    VSansv.Remove(v); // V - {v}
    SortedSet<IRBlock> S = FindReachableBlocks(cfg, v.GetIndex());

    VSansv.ExceptWith(S);

    return VSansv; // V - {v} - S
  }

  public static SortedSet<IRBlock> FindReachableBlocks(IRGraph cfg, int ignoreBlockIndex)
  {
    IRBlock head = cfg.GetGraphHead();
    SortedSet<IRBlock> reachable = new SortedSet<IRBlock>();
    reachable.Add(head);

    // if you can't use head you should ge no where
    if (head.GetIndex() == ignoreBlockIndex)
      return reachable;

    return FindReachableBlocks(reachable, head, ignoreBlockIndex);
  }

  private static SortedSet<IRBlock> FindReachableBlocks(SortedSet<IRBlock> reachable, IRBlock block, int ignoreBlockIndex)
  {
    List<IRBlock> successors = block.GetSuccessors();
    foreach (IRBlock successor in successors)
    {
      if ((!reachable.Contains(successor)) && (successor.GetIndex() != ignoreBlockIndex))
      {
        reachable.Add(successor);
        FindReachableBlocks(reachable, successor, ignoreBlockIndex);
      }
    }
    return reachable;
  }

  private void ConstructDominanceFrontierMapping()
  {
    foreach (IRBlock block in this.GetBottomUpBlockList())
    {
      this.dominanceFrontiers[block] = this.ConstructDominanceFrontierMapping(block);
    }
  }

  private SortedSet<IRBlock> ConstructDominanceFrontierMapping(IRBlock x)
  {
    // Using Ron Cytron et al Algorithm
    SortedSet<IRBlock> dfx = new SortedSet<IRBlock>();

    // DF(X)local
    foreach (IRBlock y in x.GetSuccessors())
    {
      if(!this.imediateDominator[y].Equals(x))
      {
        dfx.Add(y);
      }
    }

    // DF(X)up
    DominatorTreeNode xnode = this.blockToNodeMappings[x];
    foreach (DominatorTreeNode z in xnode.GetDescendants())
    {
      foreach (IRBlock y in this.dominanceFrontiers[z.GetBlock()])
      {
        if(!this.imediateDominator[y].Equals(x))
          dfx.Add(y);
      }
    }

    return dfx;
  }

  private List<IRBlock> GetBottomUpBlockList()
  {
    List<IRBlock> list = new List<IRBlock>();

    RecurseDownTree(list, this.rootNode);

    return list;
  }

  private void RecurseDownTree(List<IRBlock> list, DominatorTreeNode node)
  {
    foreach(DominatorTreeNode descendant in node.GetDescendants())
    {
      RecurseDownTree(list, descendant);
    }
    list.Add(node.GetBlock());
  }

}