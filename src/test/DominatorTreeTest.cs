using System;
using System.Collections.Generic;
using System.Diagnostics;
using TCDSwift;

public class DominatorTreeTest
{

  public static void Main(string [] args)
  {  
    Console.WriteLine("\n\n### Testing Dominator Tree ###");

    TestFindReachableBlocks1();
    TestFindReachableBlocks3();
    TestFindReachableBlocks4();

    Console.WriteLine("*** SUCCESS ***");
  }

  public static void TestFindReachableBlocks1()
  {
    IRGraph cfg = BuildSampleCFG();
    SortedSet<IRBlock> result = DominatorTree.FindReachableBlocks(cfg, 1);
    SortedSet<int> intResult = ConvertToIndexSet(result);

    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(1);

    if(!intResult.SetEquals(expected)) {
      throw new Exception("Reachable blocks for block 1 is " + ConvertSetToString(intResult) + " should be " + ConvertSetToString(expected));
    }
  }

  public static void TestFindReachableBlocks3()
  {
    IRGraph cfg = BuildSampleCFG();
    SortedSet<IRBlock> result = DominatorTree.FindReachableBlocks(cfg, 3);
    SortedSet<int> intResult = ConvertToIndexSet(result);

    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(1);
    expected.Add(2);

    if(!intResult.SetEquals(expected)) {
      throw new Exception("Reachable blocks for block 3 is " + ConvertSetToString(intResult) + " should be " + ConvertSetToString(expected));
    }
  }

  public static void TestFindReachableBlocks4()
  {
    IRGraph cfg = BuildSampleCFG();
    SortedSet<IRBlock> result = DominatorTree.FindReachableBlocks(cfg, 4);
    SortedSet<int> intResult = ConvertToIndexSet(result);

    SortedSet<int> expected = new SortedSet<int>();
    expected.Add(1);
    expected.Add(2);
    expected.Add(3);
    expected.Add(5);
    expected.Add(8);

    if(!intResult.SetEquals(expected)) {
      throw new Exception("Reachable blocks for block 4 is " + ConvertSetToString(intResult) + " should be " + ConvertSetToString(expected));
    }
  }

  public static void TestBuildFullTree()
  {
    // Build Dominator Tree
    IRGraph cfg = BuildSampleCFG();
    DominatorTree dominatorTree = new DominatorTree(cfg);

    // Compare Result to Expected
  }

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