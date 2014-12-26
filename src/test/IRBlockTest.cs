using System;
using System.Collections.Generic;

public class IRBlockTest
{
  public static void Main(string [] args)
  {
    IRBlock block1 = new IRBlock(1);
    block1.AppendStatement(new IRTuple(IrOp.LABEL, "l1"));
    block1.AppendStatement(new IRTupleTwoOp(IrOp.EQU, "t1", "b", "a"));

    IRBlock block2 = new IRBlock(2);
    block2.AppendStatement(new IRTupleOneOpImm<int>(IrOp.STORE, "b", 4));
    block2.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "c", "b"));
    block1.AppendStatement(new IRTupleTwoOp(IrOp.ADD, "t3", "a", "c"));
    block2.AppendStatement(new IRTuple(IrOp.JMP, "l1"));

    IRBlock block3 = new IRBlock(3);
    block3.AppendStatement(new IRTupleTwoOp(IrOp.LTE, "t2", "b", "c"));
    block3.AppendStatement(new IRTupleOneOpIdent(IrOp.JMPF, "l2", "t2"));

    IRBlock block4 = new IRBlock(4);
    block4.AppendStatement(new IRTuple(IrOp.JMP, "l3"));

    block1.AddSuccessor(block2);
    block1.AddSuccessor(block3);
    block3.AddSuccessor(block4);

    List<IRBlock> blocks = new List<IRBlock>();
    blocks.Add(block1);
    blocks.Add(block2);
    blocks.Add(block3);
    blocks.Add(block4);

    foreach (IRBlock irb in blocks)
    {
      Console.WriteLine("B" + irb.GetIndex() + ":");
      irb.PrintStatements();
      irb.PrintSuccessors();
      Console.WriteLine();
    }

  }
}
