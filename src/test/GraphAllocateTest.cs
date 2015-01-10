using System;
using System.Collections.Generic;
using TCDSwift;

public class IRGraphTest{

    public static void Main(string [] args){  
        List<IRTuple> irstream = new List<IRTuple>();

        irstream.Add(new IRTuple(IrOp.LABEL, "F$1"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "T", "R3"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "A", "R1"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "B", "R2"));
        irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "C", 0));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "D", "A"));
        irstream.Add(new IRTuple(IrOp.LABEL, "Label"));
        irstream.Add(new IRTupleTwoOp(IrOp.ADD, "C", "C", "B"));
        irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "T$1", 1));
        irstream.Add(new IRTupleTwoOp(IrOp.SUB, "D", "D", "T$1"));
        irstream.Add(new IRTupleOneOpImm<int>(IrOp.STORE, "T$2", 0));
        irstream.Add(new IRTupleTwoOp(IrOp.LTE, "T$3", "D", "T$2"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.JMPF, "Label", "T$3"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "R1", "C"));
        irstream.Add(new IRTupleOneOpIdent(IrOp.STORE, "R3", "T"));

    
        Allocate.run(irstream);
    
    }
}
