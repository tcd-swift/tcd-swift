using System;
using System.Collections.Generic;
using TCDSwift;

public class DominatorTreeTest
{

  public static void Main(string [] args)
  {  
    Console.WriteLine("\n\n### Testing Dominator Tree ###");

    TestFindReachableBlocks1();
    TestFindReachableBlocks3();
    TestFindReachableBlocks4();

    TestDomaintes3();
    TestDomaintes4();

    TestBuildFullTree();

    TestCalculatingDominanceFrontier1();
    TestCalculatingDominanceFrontier6();
    TestCalculatingDominanceFrontier9();

    Console.WriteLine("*** SUCCESS ***");
  }


  /*
   * Test find reachable blocks methods
   */
  private static void TestFindReachableBlocks(int index, SortedSet<int> expected)
  {
    IRGraph cfg = BuildSampleCFG();
    SortedSet<IRBlock> result = DominatorTree.FindReachableBlocks(cfg, index);
    SortedSet<int> intResult = ConvertToIndexSet(result);

    if(!intResult.SetEquals(expected)) {
      throw new Exception("Reachable blocks for block " + index + " is " + ConvertSetToString(intResult) + " should be " + ConvertSetToString(expected));
    }
  }

  public static void TestFindReachableBlocks1()
  {
    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(1);

    TestFindReachableBlocks(1, expected);
  }

  public static void TestFindReachableBlocks3()
  {
    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(1);
    expected.Add(2);

    TestFindReachableBlocks(3, expected);
  }

  public static void TestFindReachableBlocks4()
  {
    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(1);
    expected.Add(2);
    expected.Add(3);
    expected.Add(5);
    expected.Add(8);

    TestFindReachableBlocks(4, expected);
  }


  /*
   * Test the dominance set
   */
  private static void TestDomaintes(int index, SortedSet<int> expected)
  {
    IRGraph cfg = BuildSampleCFG();

    SortedSet<IRBlock> V = cfg.GetSetOfAllBlocks();
    SortedSet<IRBlock> VSansR = new SortedSet<IRBlock>();
    VSansR.UnionWith(V);
    VSansR.Remove(cfg.GetGraphHead());

    SortedSet<IRBlock> result = DominatorTree.CalculateDominatesSet(cfg, V, VSansR, cfg.GetBlock(index));
    SortedSet<int> intResult = ConvertToIndexSet(result);

    if(!intResult.SetEquals(expected)) {
      throw new Exception("Dominates blocks for block " + index + " is " + ConvertSetToString(intResult) + " should be " + ConvertSetToString(expected));
    }
  }

  public static void TestDomaintes3()
  {
    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(4);
    expected.Add(5);
    expected.Add(6);
    expected.Add(7);
    expected.Add(9);
    expected.Add(8);

    TestDomaintes(3, expected);
  }

  public static void TestDomaintes4()
  {
    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(6);
    expected.Add(7);
    expected.Add(9);

    TestDomaintes(4, expected);
  }


  /* 
   * Test Building Full Dominator Tree
   */
  public static void TestBuildFullTree()
  {
    // Build Dominator Tree
    IRGraph cfg = BuildSampleCFG();
    DominatorTree dominatorTree = new DominatorTree(cfg);

    // Expected
    DominatorTreeNode expected = BuildExpectedDominatorTree();

    // Compare Result to Expected
    if(!dominatorTree.GetRoot().Equals(expected)) {
      Console.WriteLine("\n\n *** RESULT ***");
      printTree(dominatorTree.GetRoot());

      Console.WriteLine("\n\n *** EXPECTED ***");
      printTree(expected);

      throw new Exception("Dominator Tree built doesn't match expected dominator tree");
    }
  }

  /* 
   * Test Building Full Dominator Tree
   */
  private static void TestCalculatingDominanceFrontier(int index, SortedSet<int> expected) {
    // Build Dominator Tree
    IRGraph cfg = BuildSampleCFG();
    DominatorTree dominatorTree = new DominatorTree(cfg);

    SortedSet<IRBlock> result = dominatorTree.GetDominanceFrontier(cfg.GetBlock(index));
    SortedSet<int> intResult = ConvertToIndexSet(result);

    if(!intResult.SetEquals(expected)) {
      throw new Exception("Dominance frontier for block " + index + " is " + ConvertSetToString(intResult) + " should be " + ConvertSetToString(expected));
    }
  }

  public static void TestCalculatingDominanceFrontier1() {
    SortedSet<int> expected = new SortedSet<int>(); 
    // should have empty dominance frontier

    TestCalculatingDominanceFrontier(1, expected);
  }

  public static void TestCalculatingDominanceFrontier6() {
    SortedSet<int> expected = new SortedSet<int>(); 
    expected.Add(9);

    TestCalculatingDominanceFrontier(6, expected);
  }

  public static void TestCalculatingDominanceFrontier9() {
    SortedSet<int> expected = new SortedSet<int>(); 
    expected.Add(3);

    TestCalculatingDominanceFrontier(9, expected);
  }

  /* 
   * Helper Methods
   */
  private static IRGraph BuildSampleCFG()
  {
    IRBlock block1 = new IRBlock(1);
    IRBlock block2 = new IRBlock(2);
    IRBlock block3 = new IRBlock(3);
    IRBlock block4 = new IRBlock(4);
    IRBlock block5 = new IRBlock(5);
    IRBlock block6 = new IRBlock(6);
    IRBlock block7 = new IRBlock(7);
    IRBlock block8 = new IRBlock(8);
    IRBlock block9 = new IRBlock(9);

    block1.AddSuccessor(block2);
    block2.AddSuccessor(block3);
    block3.AddSuccessor(block4);
    block3.AddSuccessor(block5);
    block4.AddSuccessor(block6);
    block4.AddSuccessor(block7);
    block5.AddSuccessor(block8);
    block6.AddSuccessor(block9);
    block7.AddSuccessor(block9);
    block9.AddSuccessor(block3);

    List<IRBlock> blocks = new List<IRBlock>();
    blocks.Add(block1);
    blocks.Add(block2);
    blocks.Add(block3);
    blocks.Add(block4);
    blocks.Add(block5);
    blocks.Add(block6);
    blocks.Add(block7);
    blocks.Add(block8);
    blocks.Add(block9);

    IRGraph cfg = new IRGraph(blocks);
    return cfg;
  }

  private static DominatorTreeNode BuildExpectedDominatorTree()
  {
    // this is silly replication but hey it makes life easy
    IRBlock block1 = new IRBlock(1);
    IRBlock block2 = new IRBlock(2);
    IRBlock block3 = new IRBlock(3);
    IRBlock block4 = new IRBlock(4);
    IRBlock block5 = new IRBlock(5);
    IRBlock block6 = new IRBlock(6);
    IRBlock block7 = new IRBlock(7);
    IRBlock block8 = new IRBlock(8);
    IRBlock block9 = new IRBlock(9);

    DominatorTreeNode node1 = new DominatorTreeNode(block1);
    DominatorTreeNode node2 = new DominatorTreeNode(block2);
    DominatorTreeNode node3 = new DominatorTreeNode(block3);
    DominatorTreeNode node4 = new DominatorTreeNode(block4);
    DominatorTreeNode node5 = new DominatorTreeNode(block5);
    DominatorTreeNode node6 = new DominatorTreeNode(block6);
    DominatorTreeNode node7 = new DominatorTreeNode(block7);
    DominatorTreeNode node8 = new DominatorTreeNode(block8);
    DominatorTreeNode node9 = new DominatorTreeNode(block9);

    node1.AddDescendant(node2);
    node2.AddDescendant(node3);
    node3.AddDescendant(node4);
    node3.AddDescendant(node5);
    node4.AddDescendant(node6);
    node4.AddDescendant(node7);
    node4.AddDescendant(node9);
    node5.AddDescendant(node8);
    // node4.AddDescendant(node8);

    return node1;
  }

  private static void printTree(DominatorTreeNode node) {
    Console.WriteLine("\n** Node " + node.GetBlock().GetIndex());

    foreach (DominatorTreeNode desc in node.GetDescendants()) {
      Console.Write(desc.GetBlock().GetIndex() + ", ");
    }
    Console.WriteLine();

    foreach (DominatorTreeNode desc in node.GetDescendants()) {
      printTree(desc);
    }
  }

  private static SortedSet<int> ConvertToIndexSet(SortedSet<IRBlock> blocks)
  {
    SortedSet<int> intSet = new SortedSet<int>();

    foreach (IRBlock block in blocks)
    {
      intSet.Add(block.GetIndex());
    }

    return intSet;
  }

  private static string ConvertSetToString(SortedSet<int> indexes) {
    string output = "{ ";

    foreach (int index in indexes)
    {
      output += index + ", ";
    }
    output += "}";

    return output;
  }

}