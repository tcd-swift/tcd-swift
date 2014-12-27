using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class input{

    public static List<Node> nodes;

    public static int registers;

    public static void Main(){
        registers = 12;

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
        
        nodes = new List<Node>();

        List<string> livein = new List<string>(){"R1","R2","R3"};

        Console.Write("\nLive In:");
        for(int i = 0; i < livein.Count; i++){
            Node n = getNode(livein[i]);
            Console.Write(" " + n.id);
            Node m;
            for(int j = i+1; j < livein.Count; j++){
                m = getNode(livein[j]);
                n.link(m);
            }
        }
        Console.WriteLine("\n");

        List<string> line;
        for(int i = 0; i < input.Length; i++){
            line = input[i];
            string output = string.Join(",", line.ToArray());
            Console.WriteLine(output);
            
            Node n = getNode(line[0]);
            Node m;
            for(int j = 1; j < line.Count; j++){
                m = getNode(line[j]);
                n.link(m);
            }

        }

        for(int i = 0; i < moves.GetLength(0); i++){
            Node n = getNode(moves[i,0]);
            Node m = getNode(moves[i,1]);
            n.movelink(m);
        }

        printGraph(nodes);

        List<string> results = assign(nodes, livein);
        Console.WriteLine();
        for(int i = 0; i < results.Count; i++){
            Console.WriteLine(results[i]);
        }
        Console.WriteLine();

        return;
    }

    public static void printGraph(List<Node> graph){
        Console.WriteLine();
        for(int i = 0; i < graph.Count; i++){
            Console.WriteLine(graph[i].toString());
        }
        Console.WriteLine();
    }
    
    //returns a node given an id
    public static Node getNode(string s){
        for(int i = 0; i < nodes.Count; i++){
            if(nodes[i].id == s){
                return nodes[i];
            }
        }
        //if node does not exist create and add
        Node n = new Node(s);
        nodes.Add(n);
        return n;
    }

    public static List<string> assign(List<Node> graph, List<string> livein){

        if(graph.Count <= registers){
            List<string> assigned = new List<string>();
            List<string> available = new List<string>();
            for(int i = 0; i < input.registers; i++){
                string reg = "R" + i;
                if(!livein.Contains(reg)){
                    available.Add(reg);
                }
            }

            for(int i = 0; i < graph.Count; i++){
                if(graph[i].isRegister){
                    assigned.Add(graph[i].id + " : " + graph[i].id);
                }
                else{
                    assigned.Add(graph[i].id + " : " + available[0]);
                    available.Remove(available[0]);
                }
            }
            return assigned;
        }
        return null;
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
        if(this.id != n.id && !this.contains(n.id)){    
            this.interfere.Add(n);
            n.interfere.Add(this);
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
        if(this.id != n.id && !this.related(n.id)){    
            this.move.Add(n);
            this.moveRelated = true;
            n.move.Add(this);
            n.moveRelated = true;
        }
    }
}

