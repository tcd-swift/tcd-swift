using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTreeNode
{
  private IRBlock block;
  private DominatorTreeNode ancestor;
  private List<DominatorTreeNode> descendants = new List<DominatorTreeNode>();

  public DominatorTreeNode(DominatorTreeNode ancestor, IRBlock block) {
    this.ancestor = ancestor;
    this.block = block;
  }

  public void AddDescendant(DominatorTreeNode node) {
    this.descendants.Add(node);
  }

  public DominatorTreeNode GetAncestor() {
    return this.ancestor;
  }

  public List<DominatorTreeNode> GetDescendants() {
    return this.descendants;
  }
}