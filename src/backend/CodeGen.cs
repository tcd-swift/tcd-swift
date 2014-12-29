using TCDSwift;
using System.Collections.Generic;
using System.IO;

public class CodeGen{
    public static string IRToARM(IRTuple IR){
        IRTupleOneOpIdent IROOI = IR as IRTupleOneOpIdent;
        IRTupleOneOpImm<string> IROOImm = IR as IRTupleOneOpImm<string>;
        IRTupleTwoOp IRTO = IR as IRTupleTwoOp;
        if(IR.getOp() == IrOp.NEG){
            return "Neg " + IROOI.getDest();
        }
        if(IR.getOp() == IrOp.ADD){
            return "ADD " + IR.getDest() + ", " + IRTO.getSrc1() + ", " + IRTO.getSrc2();
        }
        if(IR.getOp() == IrOp.SUB){
            return "SUB " + IR.getDest() + ", " + IRTO.getSrc1() + ", " + IRTO.getSrc2();
        }
        if(IR.getOp() == IrOp.AND){
            return "AND " + IRTO.getDest() + ", " + IRTO.getSrc1() + ", " + IRTO.getSrc2();
        }
        if(IR.getOp() == IrOp.MUL){
            if(IRTO.getDest() != IRTO.getSrc1()){
                return "MUL " + IRTO.getDest() + ", " + IRTO.getSrc1() + ", " + IRTO.getSrc2(); 
            }
            else{
                return "MUL " + IRTO.getDest() + ", " + IRTO.getSrc2() + ", " + IRTO.getSrc1(); 
            }
        }
        if(IR.getOp() == IrOp.CALL){
            string str = "STRMFD sp, {R1-R12, lr}\n";
            str += "BL " + IROOI.getDest();
            if(IR.getDest()[0] == 'R'){
                str = "MOV " + IROOI.getSrc1() + ", R0";
            }
            else{
                str = "LDR " + IROOI.getSrc1() + ", R0";
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
            str += "\nLDMFD sp, {R1-R12, pc}";
            return str;
        }
        // DIV

        if(IR.getOp() == IrOp.EQU){
            string str =  "CMP " + IRTO.getSrc1() + ", " + IRTO.getSrc2()  +'\n';
            str += "MOVEQ " + IRTO.getDest() + ", #1\n";
            str += "MOVNE " + IRTO.getDest() + ", #0";
            return str;
        }
        if(IR.getOp() == IrOp.NEQ){
            string str =  "CMP " + IRTO.getSrc1() + ", " + IRTO.getSrc2()  +'\n';
            str += "MOVEQ " + IRTO.getDest() + ", #0\n";
            str += "MOVNE " + IRTO.getDest() + ", #1";
            return str;
        }
        if(IR.getOp() == IrOp.LT){
            string str =  "CMP " + IRTO.getSrc1() + ", " + IRTO.getSrc2()  +'\n';
            str += "MOVGE " + IRTO.getDest() + ", #0\n";
            str += "MOVLT " + IRTO.getDest() + ", #1";
            return str;
        }
        if(IR.getOp() == IrOp.GT){
            string str =  "CMP " + IRTO.getSrc1() + ", " + IRTO.getSrc2()  +'\n';
            str += "MOVLE " + IRTO.getDest() + ", #0\n";
            str += "MOVGT " + IRTO.getDest() + ", #1";
            return str;
        }
        if(IR.getOp() == IrOp.JMP){
            return "JMP " + IR.getDest();
        }
        if(IR.getOp() == IrOp.JMPF){
            string str = "CMP " + IROOI.getDest() + ", #0\n";
            str += "JMPEQ " + IROOI.getSrc1();
            return str;
        }
        if(IR.getOp() == IrOp.LABEL){
            return IR.getDest() + ':';
        }
        // MOD
        if(IR.getOp() == IrOp.NOT){
            return "MVN " + IROOI.getDest() + ", " + IROOI.getSrc1();
        }
        if(IR.getOp() == IrOp.OR){
            return "ORR " + IRTO.getDest() + ", " + IRTO.getSrc1() + ", " + IRTO.getSrc2();
        }
        if(IR.getOp() == IrOp.XOR){
            return "EOR " + IRTO.getDest() + ", " + IRTO.getSrc1() + ", " + IRTO.getSrc2();
        }
        if(IR.getOp() == IrOp.STORE){
            if(IR.getDest()[0] == 'R'){
                return "LDR " + IROOI.getDest() + ", " + IROOI.getSrc1();
            }
            return "STR " + IROOI.getDest() + ", " + IROOI.getSrc1();
        }
        IR.Print();
        return "";
    }
    public static string IRListToARM(List<IRTuple> tuples){
        string str = "";
        for(int i = 0; i < tuples.Count; i++){
try{
            str += IRToARM(tuples[i]) + '\n';
}
catch(System.NullReferenceException e){ tuples[i].Print(); }
        }
        return str;
    }
    public static int IRListToArmFile(List<IRTuple> tuples, string filename){
        string output = IRListToARM(tuples);
        StreamWriter writer = new StreamWriter(filename);
        writer.Write(output);
        writer.Close();
        return 1;
    }
}
