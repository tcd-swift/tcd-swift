using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TCDSwift{

public class Allocate{

    public static Graph graph;
    
    //graph construction variables
    public static int registers = 12;
    public static List<string> livein;
    public static List<List<string>> input;
    
    public static List<IRTuple> run(List<IRTuple> irstream){
        List<string> live;
        List<List<string>> interfereList;
        List<IRTuple> temp;
        
        List<string> spill = new List<string>();
        List<List<string>> results = null;
        Dictionary<string,string> dict;

        while(results == null){
            int i = 0;
            temp = new List<IRTuple>();
            Console.WriteLine("Spill count: " + spill.Count);
            foreach (IRTuple irt in irstream){
                dict = new Dictionary<string,string>();
                HashSet<string> defined = irt.GetDefinedVars();
                HashSet<string> used = irt.GetUsedVars();
                for(int j = 0; j < spill.Count; j++){
                    dict.Add(spill[j],"#t"+i);
                    i++;
                    if(used.Contains(spill[j])){
                        temp.Add(new IRTupleOneOpIdent(IrOp.STORE, dict[spill[j]], "MEM["+spill[j]+"]"));
                    }
                }
                temp.Add(irt.TranslateNames(dict));
                for(int j = 0; j < spill.Count; j++){
                    if(defined.Contains(spill[j])){
                        temp.Add(new IRTupleOneOpIdent(IrOp.STORE, "MEM["+spill[j]+"]", dict[spill[j]]));
                    }
                }   
            }
            irstream = temp;
            
            //magic live variable analysis provided by Orla
            IRGraph irgraph = new IRGraph(irstream);
            irgraph.ComputeLiveness(out live, out interfereList);
        
            takeInput(interfereList, live);
            build();
            
            spill = new List<string>();
            results = simplify(graph, spill);
        }
        
        
        dict = new Dictionary<string,string>();
        for(int i = 0; i < results.Count; i++){
            dict.Add(results[i][0], results[i][1]);
        }
        
        Console.WriteLine();
        foreach(var kvp in dict) {
            Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
        }
        Console.WriteLine();
        
        List<IRTuple> irstream_out = new List<IRTuple>();
        foreach (IRTuple irt in irstream){
            IRTuple translated = irt.TranslateNames(dict);
            irstream_out.Add(translated);
        }
        return irstream_out;
    }

    public static void takeInput(List<List<string>> inputIn, List<string> live){
        input = inputIn;
        livein = live;
        graph = new Graph();
        Console.WriteLine("Registers = " + registers);
    }
    
    //build graph with current construction variables
    public static void build(){
        
        //remove anything that isn't an ident from input but leave as isolated node in graph
        Regex literal = new Regex(@"^[0-9]+");
        Regex str = new Regex(@"^"".+""");
        Regex memory = new Regex(@"[MEM.+]");
        
        for(int i = livein.Count-1; i >= 0; i--){
            if(literal.IsMatch(livein[i]) || str.IsMatch(livein[i])){
                graph.getNode(livein[i]);
                livein.Remove(livein[i]);
            }
        }
        for(int i = input.Count-1; i >= 0; i--){
            if(input[i][0] == ""){
                input.Remove(input[i]);
            }
            else if(literal.IsMatch(input[i][0]) || str.IsMatch(input[i][0]) || memory.IsMatch(input[i][0])){
                graph.getNode(input[i][0]);
                input.Remove(input[i]);
            }
            else{
                for(int j = input[i].Count-1; j > 0; j--){
                    if(literal.IsMatch(input[i][j]) || str.IsMatch(input[i][j]) || memory.IsMatch(input[i][j])){
                        graph.getNode(input[i][j]);
                        input[i].Remove(input[i][j]);
                    }
                }
            }  
        }
        
        //set up nodes based on live in registers
        Console.Write("\nLive In:");
        for(int i = 0; i < livein.Count; i++){
            Node n = graph.getNode(livein[i]);
            Console.Write(" " + n.id);
            Node m;
            for(int j = i+1; j < livein.Count; j++){
                m = graph.getNode(livein[j]);
                n.link(m);
            }
        }
        Console.WriteLine("\n");

        //build graph using list of interferences
        List<string> line;
        for(int i = 0; i < input.Count; i++){
            line = input[i];
            string output = string.Join(",", line.ToArray());
            Console.WriteLine(output);
            if(line[0] != ""){
                Node n = graph.getNode(line[0]);
                Node m;
                    for(int j = 1; j < line.Count; j++){
                        m = graph.getNode(line[j]);
                        n.link(m);
                    }
            }
        }

        graph.print();
    }
    
    //get list of registers based on number of registers
    public static List<string> getRegisters(){
        List<string> regs = new List<string>();
        for(int i = 1; i <= Allocate.registers; i++){
            string reg = "R" + i;
            regs.Add(reg);       
        }
        return regs;
    }

    //given a partially coloured graph and a node, attempts to integrate node
    public static List<List<string>> appendToAllocated(List<List<string>> alloc, Node n){
        List<string> regs = getRegisters();
        Regex literal = new Regex(@"^[0-9]+");
        Regex str = new Regex(@"^"".+""");
        Regex memory = new Regex(@"[MEM.+]");
        
        if(literal.IsMatch(n.id)){
            alloc.Add(new List<string>(){n.id,"="+n.id});
            return alloc;
        }
        else if(str.IsMatch(n.id) || memory.IsMatch(n.id)){
            alloc.Add(new List<string>(){n.id, n.id});
            return alloc;
        }
        for(int i = 0; i < alloc.Count; i++){
            if(n.interferes(alloc[i][0])){
                regs.Remove(alloc[i][1]);
            }
        }
        if(regs.Count > 0){
            alloc.Add(new List<string>(){n.id,regs[0]});
            return alloc;
        }
        return null;
    }

    public static List<List<string>> simplify(Graph graph, List<string> spill){
        //conditions to start colouring graph
        if(graph.Count == 1 || graph.isRegisters()){
            return assign(graph);
        }

        List<List<string>> result = new List<List<string>>();
        List<int> degrees = new List<int>();
        Node n;
        
        for(int i = 0; i < graph.Count; i++){
            if(graph.Get(i).isRegister){
                degrees.Add(-1); //causes nodes that are already registers to be ignored when simplifying
            }
            else{
                degrees.Add(graph.Get(i).getDegree());
            }
        }
        
        //define degree that node must be at (or less than) to be simplified
        int k = Allocate.registers - 1;
        while(k >= 0){
            if(degrees.Contains(k)){
                int index = degrees.IndexOf(k);
                n = graph.Get(index);
                graph.Remove(n);
                result = simplify(graph, spill);
                if(result == null){
                    return null;
                }
                graph.Add(n);
                return appendToAllocated(result, n);
            }
            k--;
        }
        n = graph.GetHighestDegreeNode();
        graph.Remove(n);
        spill.Add(n.id);
        result = simplify(graph, spill);
        if(result == null){
            return null;
        }
        graph.Add(n);
        result = appendToAllocated(result, n); //you know, let's just try to colour the node anyway
        if(result != null){
            spill.Remove(n.id);
        }
        return result;
    }

    public static List<List<string>> assign(Graph graph){
        
        List<List<string>> assigned = new List<List<string>>();
        List<string> available = new List<string>();
        
        Regex literal = new Regex(@"^[0-9]+");
        Regex str = new Regex(@"^"".+""");
        Regex memory = new Regex(@"[MEM.+]");
        
        //build list of available registers
        for(int i = 1; i <= Allocate.registers; i++){
            string reg = "R" + i;
            if(!livein.Contains(reg)){
                available.Add(reg);
            }
        }
        
        //assign registers
        for(int i = 0; i < graph.Count; i++){
            Node n = graph.Get(i);
            if(n.isRegister || str.IsMatch(n.id) || memory.IsMatch(n.id)){
                assigned.Add(new List<string>(){n.id,n.id});
            }
            else if(literal.IsMatch(n.id)){
                assigned.Add(new List<string>(){n.id,"="+n.id});
            }
            else{
                assigned.Add(new List<string>(){n.id,available[0]});
                available.Remove(available[0]);
            }
        }
        return assigned;
    }
}

public class Graph{
    public List<Node> nodes;
    public int Count;
    
    public Graph(){
        nodes = new List<Node>();
        this.Count = 0;
    }
    
    public Node Get(int i){
        return this.nodes[i];
    }
    
    public void Add(Node n){
        this.nodes.Add(n);
        for(int i = 0; i < n.interfere.Count; i++){
            if(nodes.Contains(n.interfere[i])){
                n.interfere[i].link(n);
            }
        }
        Count++;
    }
    
    public void Remove(Node n){
        this.nodes.Remove(n);
        for(int i = 0; i < this.nodes.Count; i++){
            nodes[i].Remove(n);
        }
        Count--;
    }
    
    public void print(){
        Console.WriteLine();
        for(int i = 0; i < this.nodes.Count; i++){
            Console.WriteLine(this.nodes[i].toString());
        }
        Console.WriteLine();
    }
    
    public bool isRegisters(){
        for(int i = 0; i < this.nodes.Count; i++){
            if(!this.nodes[i].isRegister){
                return false;
            }
        }
        return true;
    }
    
    //returns a node given an id
    public Node getNode(string s){
        for(int i = 0; i < this.nodes.Count; i++){
            if(this.nodes[i].id == s){
                return this.nodes[i];
            }
        }
        //if node does not exist create and add
        Node n = new Node(s);
        this.Add(n);
        return n;
    }
    
    public Node GetHighestDegreeNode(){
        Node n;
        int index = 0;
        while(nodes[index].isRegister){
            index++;
        }
        n = nodes[index];
        for(int i = 1; i < this.nodes.Count; i++){
            if(!this.nodes[i].isRegister && this.nodes[i].getDegree() > n.getDegree()){
                n = this.nodes[i];
            }
        }
        return n;
    }
}

public class Node{
    public string id;
    public List<Node> interfere;
    public bool isRegister;

    public Node(string ident){
        this.id = ident;
        this.interfere = new List<Node>();
        Regex regex = new Regex(@"R\d?\d");
        this.isRegister = regex.IsMatch(id);
    }

    public int getDegree(){
        return this.interfere.Count;
    }
    
    public void Remove(Node n){
        interfere.Remove(n);
    }

    public string toString(){
        string output = "";

        output = output + this.id + ":";
        for(int i = 0; i < this.interfere.Count; i++){
            output = output + " " + this.interfere[i].id;
        }
        output = output + ", isRegister: " + isRegister;
        return output;
    }

    public bool interferes(string s){
        for(int i = 0; i < this.interfere.Count; i++){
            if(this.interfere[i].id == s){
                return true;
            }
        }
        return false;
    }

    public void link(Node n){
        if(this.id != n.id){
            if(!this.interferes(n.id)){    
                this.interfere.Add(n);
            }
            if(!n.interferes(this.id)){
                n.interfere.Add(this);
            }
        }
    }
}
}
