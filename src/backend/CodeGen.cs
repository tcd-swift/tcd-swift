using TCDSwift;
using System.Collections.Generic;
public class CodeGen{
    public static string IRToARM(IRTuple IR){
        if(IR.getOp() == IrOp.NEG){
            return "Neg " + ((IRTupleOneOp)IR).getDest();
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
        }
        if(IR.getOp() == IrOp.AND){
            return "AND " + ((IRTupleTwoOp)IR).getDest() + ", " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2();
        }
        if(IR.getOp() == IrOp.MUL){
            if(IR.getDest != IR.getSrc1){
                return "MUL " + ((IRTupleTwoOp)IR).getDest() + ", " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2(); 
            }
            else{
                return "MUL " + ((IRTupleTwoOp)IR).getDest() + ", " + ((IRTupleTwoOp)IR).getSrc2() + ", " + ((IRTupleTwoOp)IR).getSrc1(); 
            }
        }
        if(IR.getOp() == IrOp.CALL){
            string str = "STRMFD sp, {R1-R12, lr}\n";
            str += "BL " + ((IRTupleOneOp)IR).getDest();
            if(IR.getDest()[0] == 'R'){
                str = "MOV " + ((IRTupleOneOp)IR).getSrc1() + ", R0";
            }
            else{
                str = "LDR " + ((IRTupleOneOp)IR).getSrc1() + ", R0";
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
            string str =  "CMP " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2()  +'\n';
            str += "MOVEQ " + ((IRTupleTwoOp)IR).getDest() + ", #1";
            str += "MOVNE " + ((IRTupleTwoOp)IR).getDest() + ", #0";
            return str;
        }
        if(IR.getOp() == IrOp.NEQ){
            string str =  "CMP " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2()  +'\n';
            str += "MOVEQ " + ((IRTupleTwoOp)IR).getDest() + ", #0";
            str += "MOVNE " + ((IRTupleTwoOp)IR).getDest() + ", #1";
            return str;
        }
        if(IR.getOp() == IrOp.JMP){
            return "JMP " + ((IRTupleOneOp)IR).getDest();
        }
        if(IR.getOp() == IrOp.JMPF){
            string str = "CMP " + ((IRTupleTwoOp)IR).getDest() + ", #0";
            str += "JMPEQ " + ((IRTupleTwoOp)IR).getSrc1();
            return str;
        }
        if(IR.getOp() == IrOp.LABEL){
            return IR.getDest() + ':';
        }
        // MOD
        if(IR.getOp() == IrOp.NOT){
            return "MVN " + ((IRTupleOneOp)IR).getDest() + ", " + ((IRTupleOneOp)IR).getSrc1();
        }
        if(IR.getOp() == IrOp.OR){
            return "ORR " + ((IRTupleTwoOp)IR).getDest() + ", " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2();
        }
        if(IR.getOp() == IrOp.XOR){
            return "EOR " + ((IRTupleTwoOp)IR).getDest() + ", " + ((IRTupleTwoOp)IR).getSrc1() + ", " + ((IRTupleTwoOp)IR).getSrc2();
        }
        if(IR.getOp() == IrOp.STORE){
            if(IR.getDest()[0] == 'R'){
                return "LDR " + ((IRTupleOneOp)IR).getDest()[0] + ", " + ((IRTupleOneOp)IR).getSrc1();
            }
            return "STR " + ((IRTupleOneOp)IR).getDest() + ", " + ((IRTupleOneOp)IR).getSrc1();
        }
        return "";
    }
    public static string IRListToARM(List<IRTuple> tuples){
        str = "";
        for(int i = 0; i < tuples.Length(); i++){
            str += IRToARM(tuples[i]) + '\n';
        }
        return str;
    }
    public static int IRListToArmFile(List<IRTuple> tuples, string filename){
        string output = IRListToArm(tuples);
        StreamWriter writer = new StreamWriter(filename);
        writer.Write(output);
        write.Close();
        return 1;
    }
}
