using System;
public class IRTupleTest
{
  public static void Main(string [] args)
  {
    IRTuple[] tuples = {new IRTuple(IrOp.ALLOC, "result"),
                        new IRTupleOneOpIdent(IrOp.STORE, "result", "a"),
                        new IRTupleOneOpImm<int>(IrOp.STORE, "b", 4),
                        new IRTupleOneOpIdent(IrOp.STORE, "c", "b"),
                        new IRTupleTwoOp(IrOp.ADD, "a", "b", "c")};

    foreach (IRTuple irt in tuples)
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
}
