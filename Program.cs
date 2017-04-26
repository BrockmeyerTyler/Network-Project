using System;
using System.Collections.Generic;
using System.IO;

namespace Network_Project
{
	class Program
	{
        const string GRAPH_FILE = "Kdl.gml";
		const int VERTEX_COUNT = 754;
		const int EDGE_COUNT = 899;
		const int MIN_CAPACITY = 1;
		const int MAX_CAPACITY = 20;
		const int SEED = 1;

        static void Main(string[] args)
		{
			StreamWriter outputRandom = new StreamWriter("output_random.txt");
			StreamWriter outputHighestFlow = new StreamWriter("output_highestflow.txt");
			StreamWriter outputMincut = new StreamWriter("output_mincut.txt");


			// the number of links generated from the source node and to the target node
			int K = 50;

			// studying the efficiency of the selected algorithms.
			Console.WriteLine("Studying the efficiency of the selected algorithms.\n\t|K = " + K);
			for(int j = 0; j < 2; j++)
			{
				for(int i = 0; i < 2; i++)
				{
					// build the graph using the file provided in the project description.
					NetworkGraph graph = new NetworkGraph(GRAPH_FILE, VERTEX_COUNT, EDGE_COUNT, MIN_CAPACITY, MAX_CAPACITY, K, SEED);

					// fill the graph's flow to achieve the max flow.
					graph.FillGraph();
					
					// i = 0: Random Destruction
					// i = 1: Planned Destruction
					// j = 0: Static Routing
					// j = 1: Dynamic routing
					if(i == 0)
					{
						outputRandom.WriteLine(j == 1 ? "Dynamic" : "Static");
						DestroyAtRandom(ref graph, j == 1, outputRandom);
					}
					else
					{
						if(j == 0)
							DestroyHighestFlowing(ref graph, outputHighestFlow);
						else
							DestroyMinimumCut(ref graph, outputMincut);
					}
					
				}
			}

			// studying the impact of K.
			Console.WriteLine("\n\nStudying the impact of K:");
			for(K = 30; K <= 60; K++)
			{
				Console.WriteLine("\t|K = " + K);
				outputRandom.WriteLine("|K = " + K);
				outputHighestFlow.WriteLine("|K = " + K);
				outputMincut.WriteLine("|K = " + K);
				for(int j = 0; j < 2; j++)
				{
					for(int i = 0; i < 2; i++)
					{
						// build the graph using the file provided in the project description.
						NetworkGraph graph = new NetworkGraph(GRAPH_FILE, VERTEX_COUNT, EDGE_COUNT, MIN_CAPACITY, MAX_CAPACITY, K, SEED);

						// fill the graph's flow to achieve the max flow.
						graph.FillGraph();
					
						// i = 0: Random Destruction
						// i = 1: Planned Destruction
						// j = 0: Static Routing
						// j = 1: Dynamic routing
						if(i == 0)
						{
							outputRandom.WriteLine(j == 1 ? "Dynamic" : "Static");
							DestroyAtRandom(ref graph, j == 1, outputRandom);
						}
						else
						{
							if(j == 0)
								DestroyHighestFlowing(ref graph, outputHighestFlow);
							else
								DestroyMinimumCut(ref graph, outputMincut);
						}
					
					}
				}
			}

			outputRandom.Flush();
			outputHighestFlow.Flush();
			outputMincut.Flush();

            // pause so i can read the output.
            Console.ReadLine();
        }

		static void PrintGraphInfo(NetworkGraph graph, int t, StreamWriter write)
		{
			write.WriteLine(t + "\t" + graph.maxFlow);
		}

		static void DestroyAtRandom(ref NetworkGraph graph, bool isDynamic, StreamWriter write)
		{
			// set up a list containing the indeces of the possible-to-destroy edges.
			List<int> choices = new List<int>(EDGE_COUNT);
			for(int i = 0; i < EDGE_COUNT; i++)
			{
				choices.Add(i);
			}
			int count = choices.Count;

			int halfFlowTime = -1;
			int totalFlowMaximum = graph.maxFlow;

			Console.WriteLine("Beginning 'DestroyAtRandom()'");

			// loop until the maximum flow has been reduced to 0.
			int t = 1;
			while(graph.maxFlow != 0)
			{
				// get a random number from 0 to the number of possible edge choices
				Random random = new Random(DateTime.Now.Millisecond);
				int rand = random.Next(0, count);

				// select the int at that index as the chosen index of the edge to destroy
				int choice = choices[rand];

				// 'delete' the element that we chose (move the last element to the index that we chose, then reduce the count)
				choices[rand] = choices[count - 1];
				count--;

				// destroy the link, updating the other nodes in the graph as needed.
				graph.DestroyLink(graph.edges[choice]);

				// if the routing method is 'dynamic', then recalculate the flow path.
				if(isDynamic)
					graph.FillGraph();

				if(halfFlowTime == -1 && graph.maxFlow < 0.5f * totalFlowMaximum)
					halfFlowTime = t;
					
				PrintGraphInfo(graph, t, write);
				t++;
			}
			Console.WriteLine("'DestroyAtRandom()' complete. |Max flow: " + graph.maxFlow + " |Half flow time: " + halfFlowTime);
			write.WriteLine("MaxFlow: " + graph.maxFlow + " Half flow time: " + halfFlowTime + "\n");
		}

		static void DestroyHighestFlowing(ref NetworkGraph graph, StreamWriter write)
		{
			// get the list of all edges and sort it by maximum flow.
			List<NetworkLink> edges = new List<NetworkLink>();
			for(int i = 0; i < EDGE_COUNT; i++)
				edges.Add(graph.edges[i]);
			edges.Sort((x, y) => -x.flow.CompareTo(y.flow));
			
			int halfFlowTime = -1;
			int totalFlowMaximum = graph.maxFlow;

			Console.WriteLine("Beginning 'DestroyHighestFlowing()'");

			// destroy each edge in order.
			int t = 0;
			int j = 0;
			while(graph.maxFlow > 0)
			{
				while(edges[j].flow == 0)
					j++;
				graph.DestroyLink(edges[j]);
				j++;
				
				if(halfFlowTime == -1 && graph.maxFlow < 0.5f * totalFlowMaximum)
					halfFlowTime = t;

				PrintGraphInfo(graph, t, write);
				t++;
			}
			Console.WriteLine("'DestroyHighestFlowing()' complete. |Max flow: " + graph.maxFlow + " |Half flow time: " + halfFlowTime);
			write.WriteLine("MaxFlow: " + graph.maxFlow + " Half flow time: " + halfFlowTime + "\n");
		}

		static List<NetworkLink> FindMinimumCut(NetworkNode source)
		{
			List<NetworkLink> minCut = new List<NetworkLink>();
			List<NetworkNode> nodes = new List<NetworkNode>();

			// add targets of the source node where the link between them has at least 1 flow.
			foreach(NetworkLink l in source.linksOut)
			{
				if(l.flow > 0)
					nodes.Add(l.target);
			}

			for(int i = 0; i < nodes.Count; i++)
			{
				foreach(NetworkLink l in nodes[i].linksOut)
				{
					// if the flow in the link is not 0 and is not full,
					// and if the target of the link has not already been added to the node list, add it.
					if(l.flow != 0 && l.flow != l.capacity && !nodes.Contains(l.target))
					{
						nodes.Add(l.target);
					}

					// otherwise, if the link is full, then the link is part of the minimum cut.
					else if(l.flow == l.capacity)
					{
						minCut.Add(l);
					}
				}
			}
			return minCut;
		}

		static void DestroyMinimumCut(ref NetworkGraph graph, StreamWriter write)
		{
			// get the minimum cut and sort it by highest flow.
			List<NetworkLink> minCut = FindMinimumCut(graph.source);
			minCut.Sort((x, y) => -x.flow.CompareTo(y.flow));
			
			int halfFlowTime = -1;
			int totalFlowMaximum = graph.maxFlow;
			
			Console.WriteLine("Beginning 'DestroyMinimumCut()'.");

			int t = 0;
			foreach(NetworkLink link in minCut)
			{
				graph.DestroyLink(link);
				graph.FillGraph();
				
				if(halfFlowTime == -1 && graph.maxFlow < 0.5f * totalFlowMaximum)
					halfFlowTime = t;

				PrintGraphInfo(graph, t, write);
				t++;
			}
			Console.WriteLine("'DestroyMinimumCut()' complete. |Max flow: " + graph.maxFlow + " |Half flow time: " + halfFlowTime);
			write.WriteLine("MaxFlow: " + graph.maxFlow + " Half flow time: " + halfFlowTime + "\n");
		}


    }
}
