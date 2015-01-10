using System;
using System.Collections.Generic;
using TCDSwift;

using Ident = System.String;

public class SSA
{
  public static List<IRTuple> DoSSAOptimizations(List<IRTuple> tuples)
  {
    Console.WriteLine("** SSA: Building Control Flow Graph ...");
    IRGraph graph = new IRGraph(tuples);

    // Live variable analysis
    List<string> livein;
    List<List<string>> liveouts;
    graph.ComputeLiveness(out livein, out liveouts);

    // convert into SSA form
    Console.WriteLine("** SSA: Starting conversion into SSA form ...");
    ConvertIntoSSAForm(graph);
    Console.WriteLine("** SSA: Conversion into SSA form complete");

    // do optimizations (constant propagation, dead code elimination)
    Console.WriteLine("** SSA: Starting dead code elimination ...");
    DeadCodeElimination(graph);

    Console.WriteLine("** SSA: Starting constant propagation ...");
    ConstantPropagation(graph);

    // use Briggs method to translate out of SSA form
    Console.WriteLine("** SSA: Starting conversion out of SSA form ...");
    TranslateOutOfSSAForm(graph);

    Console.WriteLine("** SSA: Generating new IRTuple Stream");
    return graph.GenerateTupleStream();
  }

  /*
   * Translate into SSA form
   */
  private static void ConvertIntoSSAForm(IRGraph graph)
  {
    DominatorTree dominatorTree = new DominatorTree(graph);
    InsertPhiFunctions(graph, dominatorTree);
    RenameVariables(graph, dominatorTree);
  }

  private static void InsertPhiFunctions(IRGraph graph, DominatorTree dominatorTree)
  {
    foreach (IRBlock block in graph.GetSetOfAllBlocks())
    {
      Console.WriteLine();
      Console.WriteLine("***");
      Console.WriteLine("Block: " + block.GetIndex());
      block.PrintStatements();

      Console.Write("Predecessors: ");
      foreach (IRBlock pred in graph.GetPredecessors(block))
        Console.Write(pred.GetIndex() + ", ");
      Console.WriteLine();

      Console.Write("Live in : ");
      foreach (Ident ident in block.GetLiveIn())
        Console.Write(ident + ", ");
      Console.WriteLine();

      Console.Write("Defined vars: ");
      foreach (Ident ident in block.GetDefinedVars())
        Console.Write(ident + ", ");
      Console.WriteLine();
    }

    foreach (Ident v in graph.GetDefinedVars())
    {
      // A(v) = blocks containing an assignment to v
      HashSet<IRBlock> Av = new HashSet<IRBlock>();
      foreach (IRBlock block in graph.GetSetOfAllBlocks())
      {
        if (block.GetDefinedVars().Contains(v) && block.GetLiveIn().Contains(v))
          Av.Add(block);
      }

      // place Phi tuple for each v in the iterated dominance frontier of A(v)
      HashSet<IRBlock> needsPhiFunction = new HashSet<IRBlock>();
      foreach (IRBlock Avblock in Av)
      {
        // create set of blocks that need phi functions
        foreach (IRBlock block in dominatorTree.GetDominanceFrontier(Avblock))
        {
          needsPhiFunction.Add(block);
        }
      }

      // only want one phi function per block for each variable where appropiate
      foreach (IRBlock block in needsPhiFunction)
      {
        // Phi function should have as many arguments as it does predecessors
        List<Ident> sources = new List<Ident>();
        foreach (IRBlock b in graph.GetPredecessors(block))
          sources.Add(v);
        
        IRTupleManyOp phi = new IRTupleManyOp(IrOp.PHI, v, sources);

        block.InsertStatement(phi, 0);
        Console.WriteLine("** SSA: Inserting phi function: " + phi.toString() + " into block " + block.GetIndex());
      }
    }
  }

  private static void RenameVariables(IRGraph graph, DominatorTree dominatorTree)
  {
    // setup data structures
    Dictionary<Ident, int> count = new Dictionary<Ident, int>();
    Dictionary<Ident, Stack<int>> stack = new Dictionary<Ident, Stack<int>>();

    foreach (Ident ident in graph.GetDefinedVars())
    {
      count[ident] = 0;
      stack[ident] = new Stack<int>();
      stack[ident].Push(0);
    }

    // recursively walk the dominator tree in-order starting with the root node
    Search(graph.GetGraphHead(), dominatorTree, count, stack);
  }

  private static void Search(IRBlock block, DominatorTree dominatorTree, Dictionary<Ident, int> count, Dictionary<Ident, Stack<int>> stack)
  {
    // Algorithm from Ron Cytron et al to rename variables into SSA form
    foreach (IRTuple irt in block.GetStatements())
    {
      Console.Write("Type: " + irt.GetType() + " ");
      irt.Print();
      Console.WriteLine();

      if (irt.getOp() != IrOp.PHI) {
        foreach (Ident v in irt.GetUsedVars()) {
          Console.WriteLine("Renaming variable: " + v);
          int i = stack[v].Peek();
          RenameTupleSourcesHelper(irt, v, i); // replace uses of v with vi in statement
        }
      }

      foreach (Ident a in irt.GetDefinedVars()) {
        count[a] = count[a] + 1;
        int i = count[a];
        stack[a].Push(i);
        irt.setDest(RenameVar(a, i)); // replace definitions of a with ai in statement
      }
    }

    // rename Phi function arguments in the successors
    List<IRBlock> successors = block.GetSuccessors();
    for (int i = 0; i < successors.Count; i++)
    {
      foreach (IRTuple stmt in block.GetStatements())
      {
        if (stmt.getOp() == IrOp.PHI)
        {
          IRTupleManyOp phi = (IRTupleManyOp)stmt;
          Ident v = phi.GetSources()[i];
          int numbering = stack[v].Peek();
          phi.SetSource(i, RenameVar(v, numbering));
        }
      }
    }

    // for each descendant do the Search(X) renaming process
    DominatorTreeNode node = dominatorTree.GetNode(block);
    foreach (DominatorTreeNode descendant in node.GetDescendants())
    {
      Search(descendant.GetBlock(), dominatorTree, count, stack);
    }

    // pop stack phase
    foreach (IRTuple irt in block.GetStatements())
    { 
      foreach (Ident v in irt.GetDefinedVars())
        stack[v].Pop();
    }
  }

  private static void RenameTupleSourcesHelper(IRTuple tuple, Ident replaceIdent, int numbering)
  {
    Console.WriteLine();
    Console.Write("Before: ");
    tuple.Print();
    Console.WriteLine();

    if (tuple.GetType() == typeof(IRTupleTwoOp))
    {
      IRTupleTwoOp twoTuple = (IRTupleTwoOp)tuple;

      if (twoTuple.getSrc2() == replaceIdent)
        twoTuple.setSrc2(RenameVar(replaceIdent, numbering));

      if (twoTuple.getSrc1() == replaceIdent)
        twoTuple.setSrc1(RenameVar(replaceIdent, numbering));


    } else if (tuple.GetType() == typeof(IRTupleOneOpIdent)) {
      IRTupleOneOpIdent oneTuple = (IRTupleOneOpIdent)tuple;
      if (oneTuple.getSrc1() == replaceIdent)
        oneTuple.setSrc1(RenameVar(replaceIdent, numbering));


    } else throw new Exception("You need to deal with more types: " + tuple.GetType());

    Console.Write("After: ");
    tuple.Print();
    Console.WriteLine();
  }

  private static Ident RenameVar(Ident old, int numbering)
  {
    return old + "_ssa" + numbering;
  }

  /*
   * Optimization Methods
   */
  private static void ConstantPropagation(IRGraph graph)
  {
    
  }


  private static void DeadCodeElimination(IRGraph graph)
  {
    Dictionary<Ident, IRIdent> identAnalysis = SimpleVariableAnalysis(graph);

    Queue<Ident> worklist = new Queue<Ident>(identAnalysis.Keys);

    while (worklist.Count > 0)
    {
      Ident ident = worklist.Dequeue();
      IRIdent identObj = identAnalysis[ident];

      if (identObj.CountUsesites() == 0)
      {
        IRTuple defStatement = identObj.GetDefsite();

        if (!defStatement.HasSideEffects())
        {
          Console.WriteLine("SSA: Deleting tuple: " + defStatement.toString() + " as it is dead code");

          HashSet<Ident> usedVars = defStatement.GetUsedVars();
          foreach (Ident used in usedVars) {
            IRIdent usedObj = identAnalysis[used];
            usedObj.DeleteUsesite(defStatement);
            worklist.Enqueue(used);
          }

          graph.RemoveStatement(defStatement);
        }
      }
    }
  }

  private static Dictionary<Ident, IRIdent> SimpleVariableAnalysis(IRGraph graph)
  {
    Dictionary<Ident, IRIdent> identAnalysis = new Dictionary<Ident, IRIdent>();

    foreach (IRBlock block in graph.GetSetOfAllBlocks())
    {
      foreach (IRTuple stmt in block.GetStatements())
      {
        HashSet<Ident> usedVars = stmt.GetUsedVars();
        foreach (Ident ident in usedVars)
        {
          IRIdent identObj;
          if (identAnalysis.ContainsKey(ident)) {
            identObj = identAnalysis[ident];
          } else {
            identObj = new IRIdent(ident, stmt);
            identAnalysis[ident] = identObj;
          }

          identObj.AddUsesite(stmt);
        }

        HashSet<Ident> definedVars = stmt.GetDefinedVars();
        foreach (Ident ident in definedVars) 
        {
          if (!identAnalysis.ContainsKey(ident)) {
            IRIdent identObj = new IRIdent(ident, stmt);
            identAnalysis[ident] = identObj;
          }
        }
      }
    }

    return identAnalysis;
  }

  /*
   * Translate out of SAA form
   */
  private static void TranslateOutOfSSAForm(IRGraph graph)
  {

  }

}