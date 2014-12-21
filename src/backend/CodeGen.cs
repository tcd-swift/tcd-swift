public class CodeGen{
    public static string IRToArm(IRTuple IR){
        if(IR.op == ADD){
            return "ADD " + IR.dest + ", " + IR.src1 + ", " + IR.src2;
        }
        return "";
    }
}
