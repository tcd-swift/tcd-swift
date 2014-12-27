using System;
using System.Collections.Generic;

using Ident = System.String;

/* A basic block of IR */
public class IRBlock
{
  private int index; // The index number of this block within the graph
  private List<IRTuple> statements;
  private List<IRBlock> successors;

  public IRBlock(int ind)
  {
    this.index = ind;
    this.statements = new List<IRTuple>();
    this.successors = new List<IRBlock>();
  }

  public int GetIndex()
  {
    return this.index;
  }

  /* Forwarding functions */
  public void AppendStatement(IRTuple stat)
  {
    this.statements.Add(stat);
  }

  public void InsertStatement(IRTuple stat, int index)
  {
    this.statements.Insert(index, stat);
  }

  public void RemoveStatement(IRTuple stat)
  {
    this.statements.Remove(stat);
  }

  public void RemoveStatementAt(int index)
  {
    this.statements.RemoveAt(index);
  }

  public IRTuple GetStatement(int index)
  {
    return this.statements[index];
  }

  public IRTuple GetFirst()
  {
    return this.statements[0];
  }

  public IRTuple GetLast()
  {
    return this.statements[this.statements.Count-1];
  }

  public int CountStatements()
  {
    return this.statements.Count;
  }

  public void AddSuccessor(IRBlock block)
  {
    this.successors.Add(block);
  }

  public void RemoveSuccessor(IRBlock block)
  {
    this.successors.Remove(block);
  }

  public void RemoveSuccessorAt(int index)
  {
    this.successors.RemoveAt(index);
  }

  public IRBlock GetSuccessor(int index)
  {
    return this.successors[index];
  }

  // Return a set of all identifiers used or defined in this block
  public HashSet<Ident> GetVarNames()
  {
    HashSet<Ident> vars = new HashSet<Ident>();
    foreach (IRTuple irt in this.statements)
    {
      vars.UnionWith(irt.GetUsedVars());
      vars.UnionWith(irt.GetDefinedVars());
    }
    return vars;
  }

  // Compute event(LiveUse) and anti-event(Def) sets for this block
  public void ComputeLiveuseDef(out HashSet<Ident> liveuse, out HashSet<Ident> def)
  {
    liveuse = new HashSet<Ident>();
    def = new HashSet<Ident>();
    HashSet<Ident> used = new HashSet<Ident>();
    HashSet<Ident> defined = new HashSet<Ident>();

    foreach (IRTuple irt in this.statements)
    {
      HashSet<Ident> usedvars = irt.GetUsedVars();
      foreach (Ident ident in usedvars)
      {
        if(!defined.Contains(ident))
          liveuse.Add(ident); // Add to liveuse any variables used before they are defined
        used.Add(ident);
      }

      HashSet<Ident> definedvars = irt.GetDefinedVars();
      foreach (Ident ident in definedvars)
      {
        if(!used.Contains(ident))
          def.Add(ident); // Add to def any variables defined before they are used
        defined.Add(ident);
      }
    }
  }

  public void PrintStatements()
  {
    foreach (IRTuple irt in this.statements)
    {
      Console.Write("{" + Enum.GetName(typeof(IrOp), irt.getOp()) + ", " + irt.getDest());

      if(irt is IRTupleOneOpIdent)
        Console.Write(", " + ((IRTupleOneOpIdent)irt).getSrc1());
      else if(irt is IRTupleOneOpImm<int>)
        Console.Write(", " + ((IRTupleOneOpImm<int>)irt).getSrc1());

      if(irt is IRTupleTwoOp)
        Console.Write(", " + ((IRTupleTwoOp)irt).getSrc2());

      Console.WriteLine("}");
    }
  }

  public void PrintSuccessors()
  {
    Console.Write("Successors: ");
    foreach (IRBlock irb in this.successors)
    {
      Console.Write("B" + irb.GetIndex() + " ");
    }
    Console.WriteLine();
  }
}

