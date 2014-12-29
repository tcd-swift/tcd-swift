using System;

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
                parser.Write(arg[1]);
            }
            IRStream tuples = parser.GetStream();
        }
        else Console.WriteLine("Usage: tcdscc <program.swift> <output.asm>");
    }
}

}
