using System;
public class IRTupleTest
{
  public static void Main(string [] args)
  {
    IRTuple[] tuples = {new IRTuple(IrOp.LABEL, "l1"),
                        new IRTupleOneOpIdent(IrOp.STORE, "result", "a"),
                        new IRTupleOneOpImm<int>(IrOp.STORE, "b", 4),
                        new IRTupleOneOpIdent(IrOp.STORE, "c", "b"),
                        new IRTupleTwoOp(IrOp.ADD, "a", "b", "c")};

    foreach (IRTuple irt in tuples)
    {
      irt.Print();
    }

  }
}
