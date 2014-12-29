using System;
using System.Collections.Generic;
using TCDSwift;

public class IRGraphTest{

    public static void Main(string [] args){  
        List<IRTuple> irstream = new List<IRTuple>();

        irstream.Add(new IRTuple(IrOp.LABEL, "F$1"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "T", "R2"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "A", "R0"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "B", "R1"));
        irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "C", 0));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "D", "A"));
        irstream.Add(new IRTuple(IrOp.LABEL, "L$1"));
        irstream.Add(new IRTupleTwoOp(IrOp.ADD, "C", "C", "B"));
        irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "T$1", 1));
        irstream.Add(new IRTupleTwoOp(IrOp.SUB, "D", "D", "T$1"));
        irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "T$2", 0));
        irstream.Add(new IRTupleTwoOp(IrOp.LTE, "T$3", "D", "T$2"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.JMPF, "L$1", "T$3"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "R0", "C"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "R2", "T"));

        IRGraph graph = new IRGraph(irstream);

        List<string> livein;
        List<List<string>> liveouts;
        graph.ComputeLiveness(out livein, out liveouts);
    
        Dictionary<string,string> registerAllocation = Allocate.run(liveouts, livein);
    
        Console.WriteLine();
        foreach(var kvp in registerAllocation) {
            Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
        }
        Console.WriteLine();
    
    }
}
