using System;
using System.Collections.Generic;
using TCDSwift;

using Ident = System.String;

public class SSA
{
  public static void DoSSAOptimizations(IRGraph graph) {
    // convert into SSA form
    ConvertIntoSSAForm(graph);

    // do optimizations (constant propagation, dead code elimination)
    DeadCodeElimination(graph);
    ConstantPropagation(graph);

    // use Briggs method to translate out of SSA form
    TranslateOutOfSSAForm(graph);
  }

  /*
   * Translate into SSA form
   */
  private static void ConvertIntoSSAForm(IRGraph graph) {
    DominatorTree dominatorTree = new DominatorTree(graph);
    InsertPhiFunctions(graph, dominatorTree);
    RenameVariables(graph, dominatorTree);
  }

  private static void InsertPhiFunctions(IRGraph graph, DominatorTree dominatorTree) {
    foreach (Ident v in graph.GetDefinedVars()) {
      // A(v) = blocks containing an assignment to v
      HashSet<IRBlock> Av = new HashSet<IRBlock>();
      foreach (IRBlock block in graph.GetSetOfAllBlocks()) {
        if (block.GetDefinedVars().Contains(v)) {
          Av.Add(block);
        }
      }

      // place Phi tuple for each v in the iterated dominance frontier of A(v)
      foreach (IRBlock Avblock in Av) {
        foreach (IRBlock block in dominatorTree.GetDominanceFrontier(Avblock)) {
          // Phi function should have as many arguments as it does predecessors
          List<Ident> sources = new List<Ident>();
          foreach (IRBlock b in graph.GetPredecessors(block)) {
            sources.Add(v);
          }
          IRTupleManyOp phi = new IRTupleManyOp(IrOp.PHI, v, sources);

          block.InsertStatement(phi, 0);
        }
      }
    }
  }

  private static void RenameVariables(IRGraph graph, DominatorTree dominatorTree) {
    // setup data structures
    Dictionary<Ident, int> count = new Dictionary<Ident, int>();
    Dictionary<Ident, Stack<int>> stack = new Dictionary<Ident, Stack<int>>();

    foreach (Ident ident in graph.GetDefinedVars()) {
      count[ident] = 0;
      stack[ident] = new Stack<int>();
      stack[ident].Push(0);
    }

    // recursively walk the dominator tree in-order starting with the root node
    Search(graph.GetGraphHead(), dominatorTree, count, stack);
  }

  private static void Search(IRBlock block, DominatorTree dominatorTree, Dictionary<Ident, int> count, Dictionary<Ident, Stack<int>> stack) {
    // Algorithm from Ron Cytron et al to rename variables into SSA form
    foreach (IRTuple irt in block.GetStatements()) {
      if (irt.getOp() != IrOp.PHI) {
        foreach (Ident v in irt.GetUsedVars()) {
          int i = stack[v].Peek();
          RenameTupleSourcesHelper(irt, v, i); // replace use of v with vi in S
        }
      }

      foreach (Ident a in irt.GetDefinedVars()) {
        count[a] = count[a] + 1;
        int i = count[a];
        stack[a].Push(i);
        irt.setDest(RenameVar(a, i)); // replace definition of a with ai in S
      }
    }

    // rename Phi function arguments in the successors
    List<IRBlock> successors = block.GetSuccessors();
    for (int i = 0; i < successors.Count; i++) {
      foreach (IRTuple stmt in block.GetStatements()) {
        if (stmt.getOp() == IrOp.PHI) {
          IRTupleManyOp phi = (IRTupleManyOp)stmt;
          Ident v = phi.GetSources()[i];
          int numbering = stack[v].Peek();
          phi.SetSource(i, RenameVar(v, numbering));
        }
      }
    }

    // for each descendant do the Search(X) renaming process
    DominatorTreeNode node = dominatorTree.GetNode(block);
    foreach (DominatorTreeNode descendant in node.GetDescendants()) {
      Search(descendant.GetBlock(), dominatorTree, count, stack);
    }

    // pop stack
    foreach (IRTuple irt in block.GetStatements()) { 
      foreach (Ident v in irt.GetDefinedVars()) { // ????
        stack[v].Pop();
      }
    }
  }

  private static void RenameTupleSourcesHelper(IRTuple tuple, Ident replaceIdent, int numbering) {
    Console.WriteLine("Before");
    tuple.Print();

    if (tuple.GetType() == typeof(IRTupleTwoOp)) {
      IRTupleTwoOp twoTuple = (IRTupleTwoOp)tuple;

      if (twoTuple.getSrc2() == replaceIdent) {
        twoTuple.setSrc2(RenameVar(replaceIdent, numbering));
      }

      if (twoTuple.getSrc1() == replaceIdent) {
        twoTuple.setSrc1(RenameVar(replaceIdent, numbering));
      }

    } else if (tuple.GetType() == typeof(IRTupleOneOpIdent)) {
      IRTupleOneOpIdent oneTuple = (IRTupleOneOpIdent)tuple;
      if (oneTuple.getSrc1() == replaceIdent) {
        oneTuple.setSrc1(RenameVar(replaceIdent, numbering));
      }

    } else {
      throw new Exception("You need to deal with more types: " + tuple.GetType());
    }

    Console.WriteLine("After");
    tuple.Print();
  }

  private static Ident RenameVar(Ident old, int numbering) {
    return old + "_ssa" + numbering;
  }

  /*
   * Optimization Methods
   */
  private static void DeadCodeElimination(IRGraph ssa_form) {
  }

  private static void ConstantPropagation(IRGraph ssa_form) {
  }

  /*
   * Translate out of SAA form
   */
  private static void TranslateOutOfSSAForm(IRGraph graph) {
  }

}