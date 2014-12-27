public class CodeGen{
    public static string IRToARM(IRTuple IR){
        if(IR.getOp() == IrOp.NEG){
            return "Neg " + IR.getDest();
        }
        if(IR.getOp() == IrOp.ADD){
            if(Object.ReferenceEquals(IR.GetType(), typeof(IRTupleTwoOp))){
                return "ADD " + IR.getDest() + ", " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2();
            }
        }
        if(IR.getOp() == IrOp.SUB){
            if(Object.ReferenceEquals(IR.GetType(), typeof(IRTupleTwoOp))){
                return "SUB " + IR.getDest() + ", " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2();
        }
        if(IR.getOp() == IrOp.AND){
            return "AND " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        // CALL
        // RET
        // DIV

        // Whats an EQU?
        if(IR.getOp() == IrOp.EQU){
            //return 
        }
        // Floating point everything
        // JMPing and Labels
        // MOD
        // NEQ
        if(IR.getOp() == IrOp.NOT){
            return "NOT " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        if(IR.getOp() == IrOp.OR){
            return "OR " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        if(IR.getOp() == IrOp.XOR){
            return "XOR " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        return "";
    }
}
