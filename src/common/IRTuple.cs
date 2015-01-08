using System;
using System.Collections.Generic;

using Ident = System.String;

namespace TCDSwift
{

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
  XOR,
  PHI
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
    IrOp.NEQ, IrOp.OR, IrOp.SUB, IrOp.XOR,
    IrOp.PHI
  };

  protected static readonly List<IrOp> hasSideEffects = new List<IrOp>()
  {
    IrOp.CALL, IrOp.RET, IrOp.STORE, IrOp.FUNC, IrOp.JMP, IrOp.JMPF, IrOp.LABEL
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

  public void setDest(Ident val) 
  {
    this.dest = val;
  }

  public bool HasSideEffects() {
    return hasSideEffects.Contains(this.op);
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

  // Return a translation of this tuple, translating its operand names using a map from old names to new names
  public virtual IRTuple TranslateNames(Dictionary<string, string> translations)
  {
    string newdest = this.dest;
    if(translations.ContainsKey(this.dest))
      newdest = translations[this.dest];
    return new IRTuple(this.op, newdest);
  }

  public virtual void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest + "}");
  }

  public virtual string toString()
  {
    return string.Format("{{{0}, {1}}}", Enum.GetName(typeof(IrOp), this.op), this.dest);
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

  public void setSrc1(Ident val)
  {
    this.src1 = val;
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

  // Return a translation of this tuple, translating its operand names using a map from old names to new names
  public override IRTuple TranslateNames(Dictionary<string, string> translations)
  {
    string newdest = this.dest;
    if(translations.ContainsKey(this.dest))
      newdest = translations[this.dest];
    string newsrc1 = this.src1;
    if(translations.ContainsKey(this.src1))
      newsrc1 = translations[this.src1];
    return new IRTupleOneOpIdent(this.op, newdest, newsrc1);
  }

  public override void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write(", " + this.src1);
    Console.Write("}");
  }

  public override string toString()
  {
    return string.Format("{{{0}, {1}, {2}}}", Enum.GetName(typeof(IrOp), this.op), this.dest, this.src1);
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

  // Return a translation of this tuple, translating its operand names using a map from old names to new names
  public override IRTuple TranslateNames(Dictionary<string, string> translations)
  {
    string newdest = this.dest;
    if(translations.ContainsKey(this.dest))
      newdest = translations[this.dest];
    return new IRTupleOneOpImm<T>(this.op, newdest, this.src1);
  }

  public override void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write(", " + this.src1);
    Console.Write("}");
  }

  public override string toString()
  {
    return string.Format("{{{0}, {1}, {2}}}", Enum.GetName(typeof(IrOp), this.op), this.dest, this.src1);
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

  public void setSrc2(Ident val)
  {
    this.src2 = val;
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

  // Return a translation of this tuple, translating its operand names using a map from old names to new names
  public override IRTuple TranslateNames(Dictionary<string, string> translations)
  {
    string newdest = this.dest;
    if(translations.ContainsKey(this.dest))
      newdest = translations[this.dest];
    string newsrc1 = this.src1;
    if(translations.ContainsKey(this.src1))
      newsrc1 = translations[this.src1];
    string newsrc2 = this.src2;
    if(translations.ContainsKey(this.src2))
      newsrc2 = translations[this.src2];      
    return new IRTupleTwoOp(this.op, newdest, newsrc1, newsrc2);
  }

  public override void Print()
  {
    Console.Write("{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest);
    Console.Write(", " + this.src1);
    Console.Write(", " + this.src2);
    Console.Write("}");
  }  

  public override string toString()
  {
    return string.Format("{{{0}, {1}, {2}, {3}}}", Enum.GetName(typeof(IrOp), this.op), this.dest, this.src1, this.src2);
  }
}

/**
 * IRTuple for multiple argument operators like Phi Functions that
 * can have a variable number of arguments
 *
 * Note: These tuples should not be used to produce machine code
 */
public class IRTupleManyOp : IRTuple
{
  protected List<Ident> sources;

  public IRTupleManyOp(IrOp irop, Ident destination, List<Ident> sources) : base(irop, destination)
  {
    this.sources = new List<Ident>();
  }

  public List<Ident> GetSources()
  {
    return this.sources;
  }

  public void SetSource(int i, Ident val) {
    this.sources[i] = val;
  }

  // Return a list of names of variables used in this tuple
  public override HashSet<Ident> GetUsedVars()
  {
    HashSet<Ident> result = new HashSet<Ident>();
    if(varusers.Contains(this.op))
    {
      foreach(Ident src in this.sources)
      {
        result.Add(src);
      }
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
    Console.Write(this.toString());
  }  

  public override string toString()
  {
    string result = "";
    result += "{" + Enum.GetName(typeof(IrOp), this.op) + ", " + this.dest;
    foreach (Ident source in this.sources)
    {
      result += ", " + source;
    }
    result += "}";

    return result;
  }
}


}
