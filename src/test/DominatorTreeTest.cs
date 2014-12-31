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

    Console.WriteLine("*** SUCCESS ***");
  }


  /*
   * Test find reachable blocks methods
   */
  public static void TestFindReachableBlocks(int index, SortedSet<int> expected)
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
  public static void TestDomaintes(int index, SortedSet<int> expected)
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
   * Test Full Tree
   */
  public static void TestBuildFullTree()
  {
    // Build Dominator Tree
    IRGraph cfg = BuildSampleCFG();
    DominatorTree dominatorTree = new DominatorTree(cfg);

    // Compare Result to Expected
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
    block9.AddSuccessor(block4);

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