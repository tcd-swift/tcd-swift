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

        if(IR.getOp() == IrOp.EQU){
            string str =  "CMP " + IR.getSrc1() + ", " + IR.getSrc2()  +'\n';
            str += "MOVEQ " + IR.getDest() + ", #1";
            str += "MOVNE " + IR.getDest() + ", #0";
            return str;
        }
        if(IR.getOp() == IrOp.EQU){
            string str =  "CMP " + IR.getSrc1() + ", " + IR.getSrc2()  +'\n';
            str += "MOVEQ " + IR.getDest() + ", #0";
            str += "MOVNE " + IR.getDest() + ", #1";
            return str;
        }
        // Floating point everything
        // JMPing and Labels
        // MOD
        // NEQ
        if(IR.getOp() == IrOp.NOT){
            return "MVN " + IR.getDest() + ", " + IR.getSrc1();
        }
        if(IR.getOp() == IrOp.OR){
            return "ORR " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        if(IR.getOp() == IrOp.XOR){
            return "EOR " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        return "";
    }
}
