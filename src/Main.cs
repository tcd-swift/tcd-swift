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
            if (parser.errors.count == 0)
            {
                parser.Write("output.ir");
            }
            IRStream tuples = parser.GetStream();
            
            IRGraph graph = new IRGraph(tuples);

            // Code Optimizations
            SSA.DoSSAOptimizations(graph);

            tuples = graph.GenerateTupleStream(); // generate new list of tuples from the optimized graph
    
            // Live variable analysis
            List<string> livein;
            List<List<string>> liveouts;
            graph.ComputeLiveness(out livein, out liveouts);

            // Register Allocation
            Dictionary<string,string> registerAllocation = Allocate.run(liveouts, livein);
    
            List<IRTuple> irstream_out = new List<IRTuple>(); 
            foreach (IRTuple irt in tuples)
            {
                IRTuple translated = irt.TranslateNames(registerAllocation);
                irstream_out.Add(translated);
            }
            // backenders should write ARM out to arg[1]
            CodeGen.IRListToArmFile(irstream_out, arg[1]);
        }
        else Console.WriteLine("Usage: tcdscc <program.swift> <output.asm>");
    }
}

}
