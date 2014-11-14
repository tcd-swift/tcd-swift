using System;
using System.Collections.Generic;

/* A basic block of IR */
public class IRBlock
{
  private string name;
  private List<IRTuple> statements;
  private List<IRBlock> successors;

  public IRBlock(string name)
  {
    this.name = name;
    this.statements = new List<IRTuple>();
    this.successors = new List<IRBlock>();
  }

  public string GetName()
  {
    return this.name;
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
      Console.Write(irb.GetName() + " ");
    }
    Console.WriteLine();
  }
}

