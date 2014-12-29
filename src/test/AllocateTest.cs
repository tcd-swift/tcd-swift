using System;
using System.Collections.Generic;

public class AllocateTest{
    
    public static void Main(){
        //test 1
        List<List<string>> input = new List<List<string>>();
        input.Add(new List<string>(){"T","R$0","R$1","T"});
        input.Add(new List<string>(){"A","R$1","A","T"});
        input.Add(new List<string>(){"B","A","B","T"});
        input.Add(new List<string>(){"C","A","B","C","T"});
        input.Add(new List<string>(){"D","B","C","D","T"});
        input.Add(new List<string>(){"C","B","C","D","T"});
        input.Add(new List<string>(){"D","B","C","D","T"});
        input.Add(new List<string>(){"D","B","C","D","T"});
        input.Add(new List<string>(){"R$0","R$0","T"});
        input.Add(new List<string>(){"R$2","R$0","R$2"});
        
        List<string> livein = new List<string>(){"R$0","R$1","R$2"};
        
        List<List<string>> results = Allocate.run(input, livein);
          
        Console.WriteLine();
        for(int i = 0; i < results.Count; i++){
            Console.WriteLine(results[i][0] + " : " + results[i][1]);
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
        for(int i = 0; i < results.Count; i++){
            Console.WriteLine(results[i][0] + " : " + results[i][1]);
        }
        Console.WriteLine();
    }
}
