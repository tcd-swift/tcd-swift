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
        if(IR.getOp() == IrOp.MUL){
            if(IR.getDest != IR.getSrc1){
                return "MUL " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2(); 
            }
            else{
                return "MUL " + IR.getDest() + ", " + IR.getSrc2() + ", " + IR.getSrc1(); 
            }
        }
        if(IR.getOp() == IrOp.CALL){
            string str = "STRMFD sp, {R1-R12, lr}\n";
            str += "BL " + IR.getDest();
            if(IR.getDest()[0] == 'R'){
                str = "MOV " + IR.getDest() + ", R0";
            }
            else{
                str = "LDR " + IR.getDest() + ", R0";
            }
            return str;
        }
        if(IR.getOp() == IrOp.RET){
            string str = "";
            if(IR.getDest()[0] == 'R'){
                str = "MOV R0, " + IR.getDest();
            }
            else{
                str = "LDR R0, " + IR.getDest();
            }
            str += "LDRMFD sp, {R1-R12, pc}\n";
            return str;
        }
        // DIV

        if(IR.getOp() == IrOp.EQU){
            string str =  "CMP " + IR.getSrc1() + ", " + IR.getSrc2()  +'\n';
            str += "MOVEQ " + IR.getDest() + ", #1";
            str += "MOVNE " + IR.getDest() + ", #0";
            return str;
        }
        if(IR.getOp() == IrOp.NEQ){
            string str =  "CMP " + IR.getSrc1() + ", " + IR.getSrc2()  +'\n';
            str += "MOVEQ " + IR.getDest() + ", #0";
            str += "MOVNE " + IR.getDest() + ", #1";
            return str;
        }
        if(IR.getOp() == IrOp.JMP){
            return "JMP " + IR.getDest();
        }
        if(IR.getOp() == IrOp.JMPF){
            string str += "CMP " + IR.getDest() + ", #0";
            str += "JMPEQ " + IR.getSrc1();
            return str;
        }
        if(IR.getOp() == IrOp.LABEL){
            return IR.getDest() + ':';
        }
        // MOD
        if(IR.getOp() == IrOp.NOT){
            return "MVN " + IR.getDest() + ", " + IR.getSrc1();
        }
        if(IR.getOp() == IrOp.OR){
            return "ORR " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        if(IR.getOp() == IrOp.XOR){
            return "EOR " + IR.getDest() + ", " + IR.getSrc1() + ", " + IR.getSrc2();
        }
        if(IR.getOp() == IrOp.STORE){
            if(IR.getDest()[0] == 'R'){
                return "LDR " + IR.getDest()[0] + ", " + IR.getSrc1();
            }
            return "STR " + IR.getDest() + ", " + IR.getSrc1();
        }
        return "";
    }
    public static string IRListToARM(IRTuple[] tuples){
        str = "";
        for(int i = 0; i < tuples.Length(); i++){
            str += IRToARM(tuples[i]) + '\n';
        }
        return str;
    }
}
