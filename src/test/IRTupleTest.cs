using System;
using System.Collections.Generic;
using TCDSwift;

using Ident = System.String;

public class IRTupleTest
{
  public static void Main(string [] args)
  {
    IRTuple[] tuples = {new IRTuple(IrOp.LABEL, "F$1"),
                        new IRTupleOneOpIdent(IrOp.STORE, "T", "R$2"),
                        new IRTupleOneOpIdent(IrOp.STORE, "A", "R$0"),
                        new IRTupleOneOpIdent(IrOp.STORE, "B", "R$1"),
                        new IRTupleOneOpImm<int>(IrOp.STORE, "C", 0),
                        new IRTupleOneOpIdent(IrOp.STORE, "D", "A"),
                        new IRTuple(IrOp.LABEL, "L$1"),
                        new IRTupleTwoOp(IrOp.ADD, "C", "C", "B"),
                        new IRTupleOneOpImm<int>(IrOp.STORE, "T$1", 1),
                        new IRTupleTwoOp(IrOp.SUB, "D", "D", "T$1"),
                        new IRTupleOneOpImm<int>(IrOp.STORE, "T$2", 0),
                        new IRTupleTwoOp(IrOp.LTE, "T$3", "D", "T$2"),
                        new IRTupleOneOpIdent(IrOp.JMPF, "L$1", "T$3"),
                        new IRTupleOneOpIdent(IrOp.STORE, "R$0", "C"),
                        new IRTupleOneOpIdent(IrOp.STORE, "R$2", "T")};

    Dictionary<string, string> translations = new Dictionary<string, string>()
    {
      {"R$0", "R$0"},
      {"R$1", "R$1"},
      {"R$2", "R$2"},
      {"D", "R$1"},
      {"C", "R$2"},
      {"B", "R$3"},
      {"A", "R$0"},
      {"T", "R$4"},
      {"T$1", "R$0"},
      {"T$2", "R$0"},
      {"T$3", "R$0"}
    };

    foreach (IRTuple irt in tuples)
    {
      irt.Print();

      HashSet<Ident> usedvars = irt.GetUsedVars();
      Console.Write("\tUses: ");
      foreach (Ident ident in usedvars)
      {
        Console.Write(ident + " ");
      }

      HashSet<Ident> definedvars = irt.GetDefinedVars();
      Console.Write("\tDefines: ");
      foreach (Ident ident in definedvars)
      {
        Console.Write(ident + " ");
      }

      Console.WriteLine();
    }

    Console.WriteLine("-------");

    foreach (IRTuple irt in tuples)
    {
      IRTuple translated = irt.TranslateNames(translations);
      translated.Print();
      Console.WriteLine();
    }

  }
}
