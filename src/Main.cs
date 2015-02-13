using System;
using System.Collections.Generic;

namespace TCDSwift  
{

using IRStream = System.Collections.Generic.List<IRTuple>;

class TCDSwift
{
  public static void Main (string[] arg)
  {
    if (arg.Length == 2)
    {
      Scanner scanner = new Scanner(arg[0]);
      Parser parser = new Parser(scanner);
      parser.Parse();
      if (parser.errors.count != 0)
      {
        Console.WriteLine("*** " + parser.errors.count + " Parsing errors ***");
        return;
      }

      parser.Write("output.ir");

      IRStream tuples = parser.GetStream();

      // Code Optimizations - 100% doesn't work yet 
      // tuples = SSA.DoSSAOptimizations(tuples);
  
      // Register Allocation  
      List<IRTuple> tuples_out = Allocate.run(tuples); 
      
      // backenders should write ARM out to arg[1]
      CodeGen.IRListToArmFile(tuples_out, arg[1]);
      Console.Write("\n");
    }
    else Console.WriteLine("Usage: tcdscc <program.swift> <output.asm>");
  }
}

}