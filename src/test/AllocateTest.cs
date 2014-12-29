using System;
using System.Collections.Generic;

public class AllocateTest{
    
    public static void Main(){
        //test 1
        List<List<string>> input = new List<List<string>>();
        input.Add(new List<string>(){"T","R1","R2","T"});
        input.Add(new List<string>(){"A","R2","A","T"});
        input.Add(new List<string>(){"B","A","B","T"});
        input.Add(new List<string>(){"C","A","B","C","T"});
        input.Add(new List<string>(){"D","B","C","D","T"});
        input.Add(new List<string>(){"C","B","C","D","T"});
        input.Add(new List<string>(){"D","B","C","D","T"});
        input.Add(new List<string>(){"D","B","C","D","T"});
        input.Add(new List<string>(){"R1","R1","T"});
        input.Add(new List<string>(){"R3","R1","R3"});
        
        List<string> livein = new List<string>(){"R0","R1","R2"};
        
        Dictionary<string,string> results = Allocate.run(input, livein);
          
        Console.WriteLine();
        foreach(var kvp in results) {
            Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
        }
        Console.WriteLine("-----------------");
        
        //test 2
        input = new List<List<string>>();
        input.Add(new List<string>(){"A","B","D"});
        input.Add(new List<string>(){"B","A","C","D","E"});
        input.Add(new List<string>(){"C","E","B","F"});
        input.Add(new List<string>(){"D","A","B","E","F"});
        input.Add(new List<string>(){"E","B","E","D","F"});
        input.Add(new List<string>(){"F","C","D","E"});
        
        livein = new List<string>();
 
        results = Allocate.run(input, livein);
          
        Console.WriteLine();
        foreach(var kvp in results) {
            Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
        }
        Console.WriteLine();
    }
}
