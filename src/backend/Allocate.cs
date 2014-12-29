using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Allocate{

    public static Graph graph;
    //graph construction variables
    public static int registers = 13;
    public static List<string> livein;
    public static List<string>[] input;

    public static void takeInput(List<string>[] inputIn, List<string> live){
        input = inputIn;
        livein = live;
        graph = new Graph();
        Console.WriteLine("Registers = " + registers);
    }
    
    //build graph with current construction variables
    public static void build(){
        
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
        for(int i = 0; i < input.Length; i++){
            line = input[i];
            string output = string.Join(",", line.ToArray());
            Console.WriteLine(output);
            
            Node n = graph.getNode(line[0]);
            Node m;
            for(int j = 1; j < line.Count; j++){
                m = graph.getNode(line[j]);
                n.link(m);
            }
        }

        graph.print();
    }
    
    //get list of registers based on number of registers
    public static List<string> getRegisters(){
        List<string> regs = new List<string>();
        for(int i = 0; i < Allocate.registers; i++){
            string reg = "R" + i;
            regs.Add(reg);       
        }
        return regs;
    }

    //given a partially coloured graph and a node, attempts to integrate node
    public static List<List<string>> appendToAllocated(List<List<string>> alloc, Node n){
        List<string> regs = getRegisters();
        for(int i = 0; i < alloc.Count; i++){
            if(n.interferes(alloc[i][0])){
                regs.Remove(alloc[i][1]);
            }
        }
        if(regs.Count > 0){
            alloc.Add(new List<string>(){n.id,regs[0]});
            return alloc;
        }
        alloc.Add(new List<string>(){n.id,"MEM["+ n.id +"]"});
        return alloc;
    }

    public static List<List<string>> simplify(Graph graph){
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
                result = simplify(graph);
                graph.Add(n);
                return appendToAllocated(result, n);
            }
            k--;
        }
        n = graph.GetHighestDegreeNode();
        graph.Remove(n);
        result = simplify(graph);
        graph.Add(n);
        return appendToAllocated(result, n); //you know, let's just try to colour the node anyway
    }

    public static List<List<string>> assign(Graph graph){
        
        List<List<string>> assigned = new List<List<string>>();
        List<string> available = new List<string>();
         
        //build list of available registers
        for(int i = 0; i < Allocate.registers; i++){
            string reg = "R" + i;
            if(!livein.Contains(reg)){
                available.Add(reg);
            }
        }
        
        //assign registers
        for(int i = 0; i < graph.Count; i++){
            if(graph.Get(i).isRegister){
                assigned.Add(new List<string>(){graph.Get(i).id,graph.Get(i).id});
            }
            else{
                assigned.Add(new List<string>(){graph.Get(i).id,available[0]});
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

