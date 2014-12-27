using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class input{

    public static Graph graph;

    public static int registers;

    public static List<string> livein;

    public static void Main(){
        registers = 8;

        List<string>[] input = new List<string>[10];
        input[0] = new List<string>(){"T","R1","R2","T"};
        input[1] = new List<string>(){"A","R2","A","T"};
        input[2] = new List<string>(){"B","A","B","T"};
        input[3] = new List<string>(){"C","A","B","C","T"};
        input[4] = new List<string>(){"D","B","C","D","T"};
        input[5] = new List<string>(){"C","B","C","D","T"};
        input[6] = new List<string>(){"D","B","C","D","T"};
        input[7] = new List<string>(){"D","B","C","D","T"};
        input[8] = new List<string>(){"R1","R1","T"};
        input[9] = new List<string>(){"R3","R1","R3"};

        string[,] moves = new string[6,2]{
            {"T","R3"},
            {"A","R1"},
            {"B","R2"},
            {"D","A"},
            {"R1","C"},
            {"R3","T"}
        };
        
        graph = new Graph();

        livein = new List<string>(){"R1","R2","R3"};

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

        for(int i = 0; i < moves.GetLength(0); i++){
            Node n = graph.getNode(moves[i,0]);
            Node m = graph.getNode(moves[i,1]);
            n.movelink(m);
        }

        graph.print();

        List<List<string>> results = assign(graph);
        
        Console.WriteLine();
        for(int i = 0; i < results.Count; i++){
            Console.WriteLine(results[i][0] + " : " + results[i][1]);
        }
        Console.WriteLine();

        return;
    }

    public static List<List<string>> simplify(Graph graph){
        List<int> degrees = new List<int>();
        for(int i = 0; i < graph.Count; i++){
            degrees.Add(graph.Get(i).getDegree());
        }
        int k = input.registers - 1;
        while(k > 0){
            if(degrees.Contains(k)){
                int index = degrees.IndexOf(k);
            }
            k--;
        }
        return null; 
    }

    public static List<List<string>> assign(Graph graph){
        Console.WriteLine("Current number of nodes: " + graph.Count);
        if(graph.Count <= registers){
        
            List<List<string>> assigned = new List<List<string>>();
            List<string> available = new List<string>();
            
            //list of free registers
            for(int i = 0; i < input.registers; i++){
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
            Node n = graph.Get(0);
            graph.Remove(n);
            graph.print();
            graph.Add(n);
            graph.print();
            return assigned;
        }
        return null;//simplify(graph);
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
        for(int i = 0; i < n.move.Count; i++){
            if(nodes.Contains(n.move[i])){
                n.move[i].movelink(n);
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
}

public class Node{
    public string id;
    public List<Node> interfere;
    public List<Node> move;
    public bool isRegister;
    public bool moveRelated;

    public Node(string ident){
        this.id = ident;
        this.interfere = new List<Node>();
        this.move = new List<Node>();
        Regex regex = new Regex(@"R\d?\d");
        this.isRegister = regex.IsMatch(id);
        this.moveRelated = false;
    }

    public int getDegree(){
        return this.interfere.Count;
    }
    
    public void Remove(Node n){
        interfere.Remove(n);
        move.Remove(n);
        if(move.Count == 0){
            moveRelated = false;
        }
    }

    public string toString(){
        string output = "";

        output = output + this.id + ":";
        for(int i = 0; i < this.interfere.Count; i++){
            output = output + " " + this.interfere[i].id;
        }
        if(this.moveRelated){
            output = output + ", moveRelated:";
            for(int i = 0; i < this.move.Count; i++){
                output = output + " " + this.move[i].id;
            }
        }
        output = output + ", isRegister: " + isRegister;
        return output;
    }

    public bool contains(string s){
        for(int i = 0; i < this.interfere.Count; i++){
            if(this.interfere[i].id == s){
                return true;
            }
        }
        return false;
    }

    public void link(Node n){
        if(this.id != n.id){
            if(!this.contains(n.id)){    
                this.interfere.Add(n);
            }
            if(!n.contains(this.id)){
                n.interfere.Add(this);
            }
        }
    }

    public bool related(string s){
        for(int i = 0; i < this.move.Count; i++){
            if(this.move[i].id == s){
                return true;
            }
        }
        return false;
    }

    public void movelink(Node n){
        if(this.id != n.id){
            if(!this.related(n.id)){
                this.move.Add(n);
                this.moveRelated = true;
            }
            if(!n.related(this.id)){
                n.move.Add(this);
                n.moveRelated = true;
            }
        }
    }
}

