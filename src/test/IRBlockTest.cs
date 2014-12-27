using System;
using System.Collections.Generic;

using Ident = System.String;

public class IRBlockTest
{
  public static void Main(string [] args)
  {
    IRBlock block1 = new IRBlock(1);
    block1.AppendStatement(new IRTuple(IrOp.LABEL, "F$1"));
    block1.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "T", "R$2"));
    block1.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "A", "R$0"));
    block1.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "B", "R$1"));
    block1.AppendStatement(new IRTupleOneOpImm<int>(IrOp.STORE, "C", 0));
    block1.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "D", "A"));

    IRBlock block2 = new IRBlock(2);
    block2.AppendStatement(new IRTuple(IrOp.LABEL, "L$1"));
    block2.AppendStatement(new IRTupleTwoOp(IrOp.ADD, "C", "C", "B"));
    block2.AppendStatement(new IRTupleOneOpImm<int>(IrOp.STORE, "T$1", 1));
    block2.AppendStatement(new IRTupleTwoOp(IrOp.SUB, "D", "D", "T$1"));
    block2.AppendStatement(new IRTupleOneOpImm<int>(IrOp.STORE, "T$2", 0));
    block2.AppendStatement(new IRTupleTwoOp(IrOp.LTE, "T$3", "D", "T$2"));
    block2.AppendStatement(new IRTupleOneOpIdent(IrOp.JMPF, "L$1", "T$3"));

    IRBlock block3 = new IRBlock(3);
    block3.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "R$0", "C"));
    block3.AppendStatement(new IRTupleOneOpIdent(IrOp.STORE, "R$2", "T"));

    block1.AddSuccessor(block2);
    block2.AddSuccessor(block2);
    block2.AddSuccessor(block3);

    List<IRBlock> blocks = new List<IRBlock>();
    blocks.Add(block1);
    blocks.Add(block2);
    blocks.Add(block3);

    foreach (IRBlock irb in blocks)
    {
      irb.ComputeLiveuseDef();
      Console.WriteLine("B" + irb.GetIndex() + ":");
      irb.PrintStatements();
      irb.PrintSuccessors();
      irb.PrintLiveuseDef();
      Console.WriteLine();
    }

  }
}
