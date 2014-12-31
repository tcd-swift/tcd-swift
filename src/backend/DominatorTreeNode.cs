using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTreeNode
{
  private IRBlock block;
  private DominatorTreeNode ancestor;
  private List<DominatorTreeNode> descendants;

  public DominatorTreeNode(DominatorTreeNode ancestor, IRBlock block)
  {
    this.ancestor = ancestor;
    this.block = block;
    this.descendants = new List<DominatorTreeNode>();
  }

  public IRBlock GetBlock()
  {
    return this.block;
  }

  public DominatorTreeNode GetAncestor()
  {
    return this.ancestor;
  }

  public void AddDescendant(DominatorTreeNode node)
  {
    this.descendants.Add(node);
  }

  public List<DominatorTreeNode> GetDescendants()
  {
    return this.descendants;
  }
}