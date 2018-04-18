using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

/*
 *	Name:			Russell Matthews
 *	Date:			4/18/18
 *	Program:		Dijkstra's Algorithm
 *	Description:	This program creates a basic node/graph classes and also creates an
 *					implementation of Dijkstra's algorithm to find the shortest distance
 *					from one node to all other nodes. The nodes in the class will also keep
 *					track of the previous node to get there so it could also draw the path
 *					to get to each node in the shortest distance
 */

namespace Assignment5 {
    class main {
        static void Main(string[] args) {
			// get input from user
			Console.WriteLine("Filename for adjacency list:");
			string filename = Console.ReadLine();

            // get the numbers from the file and store it as an array of strings
            string[] nodes = File.ReadAllLines(filename);

			// create a list to hold the numbers
			Graph graph = new Graph(nodes);

			// run Dijkstra's on the graph
			Dijkstra(ref graph, "1");

			// do the work to print the results in the desired format
			string printString = "";

			foreach (var node in graph.Nodes) {
				printString += node.PathDistance + ",";
			}

			Console.WriteLine(printString.Substring(0, printString.Length - 1));
		}

		static void Dijkstra(ref Graph graph, string start) {
			// initialize the visited nodes and the considered edges
			List<string> visited = new List<string> {start};
			List<KeyValuePair<string, string>> consideredEdges = new List<KeyValuePair<string, string>>();

			// set the starting node's path distance to 0
			graph[start].PathDistance = 0;

			// add all the starting nodes edges to the considered edges
			foreach (var edge in graph[start].GetConnectedNodes()) {
				consideredEdges.Add(new KeyValuePair<string, string>(start, edge.Key));
			}

			// while we haven't visted all possible edges
			while (consideredEdges.Count > 0) {
				// initialize shortest distance and shortest edge
				Int64 shortestDistance = Int64.MaxValue;
				KeyValuePair<string, string> shortestEdge = new KeyValuePair<string, string>();

				// for every edge we are currently considering
				foreach (var edge in consideredEdges) {
					if (!visited.Contains(edge.Value)) {
						// if the distance to get to the node would be less than the current shortest distance
						Int64 currDistance = graph[edge.Key].PathDistance + graph[edge.Key].GetDistance(edge.Value);
						if (currDistance < shortestDistance) {
							// mark that distance and edge
							shortestDistance = currDistance;
							shortestEdge = edge;
						}
					}
				}

				// once we have found a shortest distance, remove that edge from the currently considered edges
				consideredEdges.Remove(shortestEdge);

				// update the nodes distance and path values
				graph[shortestEdge.Value].PathDistance = shortestDistance;
				graph[shortestEdge.Value].PrevNode = shortestEdge.Key;

				// add the node to visited nodes
				visited.Add(shortestEdge.Value);

				// add the possible edges to the considered edges list
				foreach (var node in graph[shortestEdge.Value].GetConnectedNodes()) {
					// if we haven't visited the node that the edge goes to, add it to consideredEdges
					if (!visited.Contains(node.Key)) {
						consideredEdges.Add(new KeyValuePair<string, string>(shortestEdge.Value, node.Key));
					}
				}

				// remove all other edges that point to that node
				for (int i = consideredEdges.Count - 1; i >= 0; i--) {
					if (visited.Contains(consideredEdges[i].Value)) {
						consideredEdges.Remove(consideredEdges[i]);
					}
				}
			}
		}
    }

    public class Node {
        // name of nodes and list of connected nodes
        private string name;
        private List<KeyValuePair<string, Int64>> connectedNodes = new List<KeyValuePair<string, Int64>>();
		private Int64 pathDistance = Int64.MaxValue;
		private string prev;

        // constructor
        public Node(string nodeString = "") {
			if (nodeString == "") {
				name = "";
			} else {
				// have to test to see if this is tab delimited or space delimeted because 
				// test 3 has a file that is tab delimited where all the others are space
				// delimited
				string[] info;
				var regex = new Regex("[' ']");

				// if it has spaces
				if (regex.IsMatch(nodeString)) {
					// take the line from the file readline, turn into array
					info = nodeString.Split(' ');
				} else {
					// take the line from the file readline, turn into array
					string sep = "\t";
					info = nodeString.Split(sep.ToCharArray());
				}

				// name = the first word of the line
				name = info[0];
            
				// for all the rest of the words
				for (Int64 i = 1; i < info.Length; i++) {
					// name the current string
					string currString = info[i];

					if (currString != "") {
						// get the neighbor node and distance
						string[] connection = currString.Split(',');

						// add it to its connected nodes
						connectedNodes.Add(new KeyValuePair<string, Int64>(connection[0], Int64.Parse(connection[1])));
					}

				}
			}
        }

		// add a neighboring connected node
        public bool AddNeighbor(string nodeName, Int64 distance) {
			// create a KeyValuePair with the name and distance
            KeyValuePair<string, Int64> node = new KeyValuePair<string, Int64>(nodeName, distance);

			// if that connection is not already made, add it and return true
            if (!connectedNodes.Contains(node)) {
                connectedNodes.Add(node);
                return true;
            } else {
                return false;
            }
        }

		// remove node from connectedNodes
        public bool RemoveNode(string nodeName, Int64 distance) {
			// make KeyValuePair to test against
            KeyValuePair<string, Int64> node = new KeyValuePair<string, Int64>(nodeName, distance);

			// remove it and return whether it was successful or not
            return connectedNodes.Remove(node);
        } 

		// get the distance of the specified neighbor
        public Int64 GetDistance(string nodeName) {
			// find the node and return its distance
            foreach (var node in connectedNodes) {
                if (node.Key == nodeName) {
                    return node.Value;
                }
            }

			// return -1 if it couldn't find it
            return -1;
        }

		// get an array of the names of neighbors of the node
		public string[] GetNeighbors() {
			string[] returnArray = new string[connectedNodes.Count];

			for (int i = 0; i < connectedNodes.Count; i++) {
				returnArray[i] = connectedNodes[i].Key;
			}

			return returnArray;
		}

		// get the list of connected nodes
		public List<KeyValuePair<string, Int64>> GetConnectedNodes() => connectedNodes;

		// get/set the attributes of the node
		public string GetName() => name;
		public Int64 PathDistance { get => pathDistance; set => pathDistance = value; }
		public string PrevNode { get => prev; set => prev = value; }
	}

    public class Graph {

		// list of nodes in graph
		private List<Node> nodes = new List<Node>();

		public Graph(string[] input) {
			// create new nodes for every string and add them to the list
			foreach (string newNode in input) {
				nodes.Add(new Node(newNode));
			}
		}

		// allow access to the node by name
		public Node this[string name] {
			get {
				foreach (Node node in nodes) {
					if (node.GetName() == name) {
						return node;
					}
				}

				return new Node();
			}
		}

		// allow you to print contents of graph
		public override string ToString() {
			if (nodes.Count == 0) {
				return "";
			}

			string returnString = "[";

			for (int i = 0; i < nodes.Count; i++) {
				returnString += nodes[i].GetName();

				if (i != nodes.Count - 1) {
					returnString += ", ";
				}
			}

			returnString += "]";

			return returnString;
		}

		// gets an array of all of the nodes
		public Node[] Nodes => nodes.ToArray();

		// gets array of all names of nodes
		public string[] Names() {
			if (nodes.Count == 0) {
				return new string[0];
			}

			string[] names = new string[nodes.Count];

			for (int i = 0; i < nodes.Count; i++) {
				names[i] = nodes[i].GetName();
			}

			return names;
		}
	}
}