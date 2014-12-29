using System;
using System.Collections.Generic;

public class AllocateTest{
    
    public static void Main(){
        //test 1
        List<string>[] input = new List<string>[10];
        input[0] = new List<string>(){"T","R0","R1","T"};
        input[1] = new List<string>(){"A","R1","A","T"};
        input[2] = new List<string>(){"B","A","B","T"};
        input[3] = new List<string>(){"C","A","B","C","T"};
        input[4] = new List<string>(){"D","B","C","D","T"};
        input[5] = new List<string>(){"C","B","C","D","T"};
        input[6] = new List<string>(){"D","B","C","D","T"};
        input[7] = new List<string>(){"D","B","C","D","T"};
        input[8] = new List<string>(){"R0","R0","T"};
        input[9] = new List<string>(){"R2","R0","R2"};
        
        List<string> livein = new List<string>(){"R0","R1","R2"};
        
        Allocate.takeInput(input, livein);
        Allocate.build();
        
        List<List<string>> results = Allocate.simplify(Allocate.graph);
          
        Console.WriteLine();
        for(int i = 0; i < results.Count; i++){
            Console.WriteLine(results[i][0] + " : " + results[i][1]);
        }
        Console.WriteLine("-----------------");
        
        //test 2
        input = new List<string>[6];
        input[0] = new List<string>(){"A","B","D"};
        input[1] = new List<string>(){"B","A","C","D","E"};
        input[2] = new List<string>(){"C","E","B","F"};
        input[3] = new List<string>(){"D","A","B","E","F"};
        input[4] = new List<string>(){"E","B","E","D","F"};
        input[5] = new List<string>(){"F","C","D","E"};
        
        livein = new List<string>();
        
        
        Allocate.takeInput(input, livein);
        Allocate.build();
        
        results = Allocate.simplify(Allocate.graph);
          
        Console.WriteLine();
        for(int i = 0; i < results.Count; i++){
            Console.WriteLine(results[i][0] + " : " + results[i][1]);
        }
        Console.WriteLine();
    }
}
