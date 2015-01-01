using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTreeNode : IComparable<DominatorTreeNode>
{
  private IRBlock block;
  private SortedSet<DominatorTreeNode> descendants;

  public DominatorTreeNode(IRBlock block)
  {
    this.block = block;
    this.descendants = new SortedSet<DominatorTreeNode>();
  }

  public bool Equals(DominatorTreeNode other)
  {
    return ((this.block.GetIndex() == other.GetBlock().GetIndex()) && (this.descendants.SetEquals(other.GetDescendants())));
  }

  public int CompareTo(DominatorTreeNode other) {
    return this.block.CompareTo(other.GetBlock());
  }

  public IRBlock GetBlock()
  {
    return this.block;
  }

  public void AddDescendant(DominatorTreeNode node)
  {
    this.descendants.Add(node);
  }

  public SortedSet<DominatorTreeNode> GetDescendants()
  {
    return this.descendants;
  }

  public void printDescendants() {
    foreach (DominatorTreeNode node in this.descendants) {
      Console.WriteLine(node.GetBlock().GetIndex() + ", ");
    }
  }
}