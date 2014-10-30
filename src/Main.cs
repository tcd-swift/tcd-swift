using System;

namespace TCDSwift { // DMA

class TCDSwift { // DMA
    public static void Main (string[] arg) {
        if (arg.Length == 2) {
            Scanner scanner = new Scanner(arg[0]);
            Parser parser = new Parser(scanner);
            parser.Parse();
            if (parser.errors.count == 0) {
                parser.Write(arg[1]);
            }
            } else Console.WriteLine("Usage: tcdscc <program.swift> <output.asm>");
    }
}

}
