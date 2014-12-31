using System;
using System.Collections.Generic;
using TCDSwift;

using Ident = System.String;

/* A basic block of IR */
public class IRBlock
{
  private int index; // The index number of this block within the graph
  private List<IRTuple> statements;
  private List<IRBlock> successors;
  private HashSet<Ident> liveuse;
  private HashSet<Ident> def;
  private HashSet<Ident> livein;
  private HashSet<Ident> liveout;
  private List<List<string>> liveouts;

  public IRBlock(int ind)
  {
    this.index = ind;
    this.statements = new List<IRTuple>();
    this.successors = new List<IRBlock>();

    this.liveuse = new HashSet<Ident>();
    this.def = new HashSet<Ident>();
    this.livein = new HashSet<Ident>();
    this.liveout = new HashSet<Ident>();

    this.liveouts = new List<List<string>>();
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

  public List<IRBlock> GetSuccessors() {
    return this.successors.AsReadOnly();
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
  public void ComputeLiveuseDef()
  {
    this.def.Clear();
    this.liveuse.Clear();
    this.livein.Clear();
    this.liveout.Clear();

    HashSet<Ident> used = new HashSet<Ident>();
    HashSet<Ident> defined = new HashSet<Ident>();

    foreach (IRTuple irt in this.statements)
    {
      HashSet<Ident> usedvars = irt.GetUsedVars();
      foreach (Ident ident in usedvars)
      {
        if(!defined.Contains(ident))
          this.liveuse.Add(ident); // Add to liveuse any variables used before they are defined
        used.Add(ident);
      }

      HashSet<Ident> definedvars = irt.GetDefinedVars();
      foreach (Ident ident in definedvars)
      {
        if(!used.Contains(ident))
          this.def.Add(ident); // Add to def any variables defined before they are used
        defined.Add(ident);
      }
    }
  }

  // Update the LiveIn and LiveOut sets
  public void UpdateLiveness()
  {
    foreach (IRBlock irb in this.successors)
      this.liveout.UnionWith(irb.livein); // LiveOut_i <- Union_(j in succ(i)) LiveIn_j

    this.livein.UnionWith(this.liveuse); // LiveIn_i <- LiveUse_i + ...
    foreach (Ident ident in this.liveout){
      if(!this.def.Contains(ident)) // ... LiveOut_i . not(Def_i)
        this.livein.Add(ident);
    }
  }

  public HashSet<Ident> GetLiveIn()
  {
    return this.livein;
  }

  public HashSet<Ident> GetLiveOut()
  {
    return this.liveout;
  }

  public List<List<string>> GetLiveOuts()
  {
    return this.liveouts;
  }

  // Backward pass to determine liveness at each point in the block
  public void ComputeLiveouts()
  {
    this.liveouts.Clear();

    Stack<List<string>> reversed = new Stack<List<string>>(); // Because this is a backward pass, lists will be found in reverse
    HashSet<Ident> lo = new HashSet<Ident>(this.liveout);

    // Extract liveness information for last statement in block
    IRTuple last = this.statements[this.statements.Count-1];

    HashSet<Ident> prevdef = last.GetDefinedVars();
    string deflabel = "";
    foreach (Ident ident in prevdef)
      deflabel = ident;

    HashSet<Ident> prevused = last.GetUsedVars();

    List<string> lastliveout = new List<string>();
    lastliveout.Add(deflabel);
    foreach (Ident ident in lo)
      lastliveout.Add(ident);

    reversed.Push(lastliveout);

    Console.WriteLine();
    for(int i = this.statements.Count-2; i >= 0; i--) // Start from second-last statement, as we already have liveout for last one
    {
      IRTuple tup = this.statements[i];

      if(lo.Contains(deflabel))
        lo.Remove(deflabel); // Remove whatever is defined in the next statement

      foreach (Ident ident in prevused)
      {
        if(!lo.Contains(ident))
        lo.Add(ident); // Add whatever is used in the next statement
      }

      deflabel = "";
      prevdef = tup.GetDefinedVars();
      foreach (Ident ident in prevdef)
        deflabel = ident;

      prevused = tup.GetUsedVars();

      List<string> currentliveout = new List<string>();
      currentliveout.Add(deflabel);

      foreach (Ident ident in lo)
        currentliveout.Add(ident);
      reversed.Push(currentliveout);
    }

    while(reversed.Count > 0)
    {
      this.liveouts.Add(reversed.Pop());
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

  public void PrintLiveuseDef()
  {
    Console.Write("Variables: ");
    HashSet<Ident> vars = this.GetVarNames();
    foreach (Ident ident in vars)
      Console.Write(ident + " ");
    Console.WriteLine();

    Console.Write("LiveUse: ");
    foreach (Ident ident in liveuse)
      Console.Write(ident + " ");
    Console.WriteLine();

    Console.Write("Def: ");
    foreach (Ident ident in def)
      Console.Write(ident + " ");
    Console.WriteLine();
  }

  public void PrintLiveInOut()
  {
    Console.Write("LiveIn: ");
    foreach (Ident ident in this.livein)
      Console.Write(ident + " ");
    Console.WriteLine();

    Console.Write("LiveOut: ");
    foreach (Ident ident in this.liveout)
      Console.Write(ident + " ");
    Console.WriteLine();
  }

  public void PrintLiveouts()
  {
    Console.WriteLine("LiveOuts:");
    foreach (List<string> lo in this.liveouts)
    {
      Console.Write("\t");
      foreach (string s in lo)
        Console.Write(s + "\t");
      Console.WriteLine();
    }

  }
}
