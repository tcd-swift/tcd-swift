using System;
using System.Collections.Generic;

// Type of an Ident; leaving this as string for now
using Ident = System.String;

/*
 * The different internal types for IR tuples
 * Some of them like LABEL are not really ops
 */
public enum IrOp: int
{
  ADD,
  AND,
  CALL,
  DIV,
  EQU,
  FADD,
  FDIV,
  FMUL,
  FSUB,
  FUNC,
  GT,
  GTE,
  JMP,
  JMPF,
  LABEL,
  LOAD,
  LT,
  LTE,
  MOD,
  MUL,
  NEG,
  NEQ,
  NOT,
  OR,
  RET,
  STORE,
  SUB,
  XOR
};

/* Generalized IRTuple class */
public class IRTuple
{
  protected static readonly List<IrOp> varusers = new List<IrOp>() // The IR operators that involve use or definition of a variable
  {
    IrOp.JMPF, IrOp.NEG, IrOp.NOT,
    IrOp.STORE,
    IrOp.ADD, IrOp.AND, IrOp.DIV, IrOp.EQU, IrOp.FADD, IrOp.FDIV, IrOp.FMUL, 
    IrOp.FSUB, IrOp.GT, IrOp.GTE, IrOp.LT, IrOp.LTE, IrOp.MOD, IrOp.MUL, 
    IrOp.NEQ, IrOp.OR, IrOp.SUB, IrOp.XOR
  };

  protected IrOp op;
  protected Ident dest;

  public IRTuple(IrOp irop, Ident destination)
  {
    this.op = irop;
    this.dest = destination;
  }

  public IrOp getOp()
  {
    return this.op;
  }
  
  public Ident getDest()
  {
    return this.dest;
  }

  // Return a list of names of variables used in this tuple
  public virtual HashSet<Ident> GetUsedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    return result; // Operations without operators neither use nor assign variables; likewise for those operators with a single operand that is immediate
  }

  // Return a list of names of variables defined in this tuple
  public virtual HashSet<Ident> GetDefinedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    return result; // Operations without operators neither use nor assign variables
  }

  public virtual void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write("}");
  }
}

/* IRTuple with one operand where operand is an Ident */
public class IRTupleOneOpIdent : IRTuple
{
  protected Ident src1;

  public IRTupleOneOpIdent(IrOp irop, Ident destination, Ident source) : base(irop, destination)
  {
    this.src1 = source;
  }

  public Ident getSrc1()
  {
    return this.src1;
  }

  // Return a list of names of variables used in this tuple
  public override HashSet<Ident> GetUsedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    if(varusers.Contains(this.op))
      result.Add(this.src1);
    return result;
  }

  // Return a list of names of variables defined in this tuple
  public override HashSet<Ident> GetDefinedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    if(varusers.Contains(this.op))
      if(this.op != IrOp.JMPF) // JumpF is an exception
        result.Add(this.dest);
    return result;
  }

  public override void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write(", " + this.src1);
    Console.Write("}");
  }
}

/* IRTuple with one operand where operand is an immediate */
public class IRTupleOneOpImm<T> : IRTuple
{
  protected T src1;

  public IRTupleOneOpImm(IrOp irop, Ident destination, T source) : base(irop, destination)
  {
    this.src1 = source;
  }

  public T getSrc1()
  {
    return this.src1;
  }

  // Return a list of names of variables defined in this tuple
  public override HashSet<Ident> GetDefinedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    if(varusers.Contains(this.op))
      result.Add(this.dest);
    return result;
  }

  public override void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write(", " + this.src1);
    Console.Write("}");
  }
}

/* IRTuple with two operands */
public class IRTupleTwoOp : IRTupleOneOpIdent
{
  protected Ident src2;

  public IRTupleTwoOp(IrOp irop, Ident destination, Ident source1, Ident source2) : base(irop, destination, source1)
  {
    this.src2 = source2;
  }

  public Ident getSrc2()
  {
    return this.src2;
  }

  // Return a list of names of variables used in this tuple
  public override HashSet<Ident> GetUsedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    if(varusers.Contains(this.op))
    {
      result.Add(this.src1);
      result.Add(this.src2);
    }
    return result;
  }

  // Return a list of names of variables defined in this tuple
  public override HashSet<Ident> GetDefinedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    if(varusers.Contains(this.op))
      result.Add(this.dest);
    return result;
  }

  public override void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write(", " + this.src1);
    Console.Write(", " + this.src2);
    Console.Write("}");
  }  
}
