using System;
using System.Collections.Generic;

// Type of an IRStream; we can change this if we need something more sophisticated later
using IRStream = System.Collections.Generic.List<IRTuple>;

public class IRGraphTest
{
  public static void Main(string [] args)
  {
    IRStream irstream = new IRStream();
    irstream.Add(new IRTuple(IrOp.FUNC, "mult1"));
    irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "result", 0));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "temp", "op2"));
    irstream.Add(new IRTuple(IrOp.LABEL, "l1"));
    irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "t1", 0));
    irstream.Add(new IRTupleTwoOp(IrOp.GT, "t2", "temp", "t1"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.JMPF, "t2", "l2"));
    irstream.Add(new IRTupleTwoOp(IrOp.ADD, "result", "result", "op1"));
    irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "t3", 1));
    irstream.Add(new IRTupleTwoOp(IrOp.SUB, "temp", "temp", "t3"));
    irstream.Add(new IRTuple(IrOp.JMP, "l1"));
    irstream.Add(new IRTuple(IrOp.LABEL, "l2"));
    irstream.Add(new IRTuple(IrOp.RET, "result"));

    IRGraph graph = new IRGraph(irstream);

    graph.Print();
  }
}
