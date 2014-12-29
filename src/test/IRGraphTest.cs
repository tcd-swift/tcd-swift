using System;
using System.Collections.Generic;
using TCDSwift;

public class IRGraphTest
{
  public static void Main(string [] args)
  {
    List<IRTuple> irstream = new List<IRTuple>();

    irstream.Add(new IRTuple(IrOp.LABEL, "F$1"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "T", "R$2"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "A", "R$0"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "B", "R$1"));
    irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "C", 0));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "D", "A"));
    irstream.Add(new IRTuple(IrOp.LABEL, "L$1"));
    irstream.Add(new IRTupleTwoOp(IrOp.ADD, "C", "C", "B"));
    irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "T$1", 1));
    irstream.Add(new IRTupleTwoOp(IrOp.SUB, "D", "D", "T$1"));
    irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "T$2", 0));
    irstream.Add(new IRTupleTwoOp(IrOp.LTE, "T$3", "D", "T$2"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.JMPF, "L$1", "T$3"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "R$0", "C"));
    irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "R$2", "T"));

    IRGraph graph = new IRGraph(irstream);

    List<string> livein;
    List<List<string>> liveouts;
    graph.ComputeLiveness(out livein, out liveouts);

    graph.Print();

    Console.WriteLine("-------");

    Console.WriteLine("LiveIn:");
    foreach(string li in livein)
        Console.Write("\t" + li);
    Console.WriteLine();

    Console.WriteLine("LiveOuts:");
    foreach(List<string> los in liveouts)
    {
        foreach(string lo in los)
            Console.Write(lo + "\t");
        Console.WriteLine();
    }

  }
}
