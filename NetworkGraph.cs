using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Network_Project
{
	class NetworkGraph
	{
        public NetworkNode[] vertices; // IDs: 0 to 753
		public NetworkLink[] edges; // IDs: 0 to 898

		public NetworkNode source;
		public NetworkNode target;

		int maxFlow = 0;


		//	Creates a directed network graph from a file. Automatically sets each link's capacity using an RNG.
		//	Generates a source and target node, connecting them at maximum flow to 'K' other nodes in the graph.
		//		fileName: name of the file containing the graph data.
		//		vertextCount: the number of verteces contained in the file.
		//		edgeCount: the number of edges contained in the file.
		//		minCapacity: the minimum capacity of the links in the graph. These will be randomly generated.
		//		maxCapacity: the maximum capacity of the links in the graph. These will be randomly generated.
		//		K: the number of edges that the generated source and target nodes will have going into, and out of, respectfully.
		//		SEED: the random number generator seed.
		public NetworkGraph(string fileName, int vertexCount, int edgeCount, int minCapacity, int maxCapacity, int K, int SEED)
		{
			Random random = new Random(SEED);
            vertices = new NetworkNode[vertexCount + 2];	// plus 2 for the source and target nodes that will be generated.
            edges = new NetworkLink[edgeCount + 2 * K];		// plus 2 * K for the K edges going into and out of the source and target.

			// begin reading and parsing the file.
            using (Stream Stream = File.OpenRead(fileName))
            using (StreamReader reader = new StreamReader(Stream))
            {
                string[] sepNodes = { "node [", "edge [" };
                //Index 0 is everything before nodes, Don't need; 
                //Index nodes.Length is the edges, parsed after;
                string[] arrayStrNodes = reader.ReadToEnd().Split(sepNodes, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < arrayStrNodes.Length; i++)
                {
                    string[] arrNode = arrayStrNodes[i].Split('\n');

                    bool isEdge = false;
                    int id = -1;
                    float longitude = -1;
                    float latitude = -1;
                    string label = "";
                    int source = -1;
                    int target = -1;

                    for (int j = 1; j<arrNode.Length; j++)
                    {
                        if (arrNode[j].Length > 4)
                        {
                            
                            switch (arrNode[j][4])
                            {
                                case 'i':
                                    string strID = arrNode[j].Substring(7);
                                    if (strID.Length > 1 && strID[1] == 'e')//added length check for id 0 of nodes
                                    {
                                        isEdge = true;
                                        id = Convert.ToInt32(strID.Substring(2, strID.Length - 4));
                                    }
                                    else
                                        id = Convert.ToInt32(strID);
                                    break;
                                case 'L':
                                    if(arrNode[j][5] == 'o')//Longitude
                                    {
                                        longitude = (float)Convert.ToDouble(arrNode[j].Substring(14));
                                    }
                                    else//Latitude
                                    {
                                        latitude = (float)Convert.ToDouble(arrNode[j].Substring(13));
                                    }
                                    break;
                                case 'l':
                                    label = arrNode[j].Substring(11, arrNode[j].Length - 13);
                                    break;
                                case 's':
                                    source = Convert.ToInt32(arrNode[j].Substring(10));
                                    break;
                                case 't':
                                    target = Convert.ToInt32(arrNode[j].Substring(10));
                                    break;
                            }
                            
                        }
                    }
                    if (id > -1)
                    {
                        if (isEdge)
                        {
                            NetworkLink link = new NetworkLink(vertices[source], vertices[target]);
                            edges[id] = link;
							link.capacity = random.Next(minCapacity, maxCapacity + 1);
                        }
                        else
                        {
                            NetworkNode node = new NetworkNode(label, longitude, latitude);
                            vertices[id] = node;
                        }
                    }
                }
            }

            //Declaring the source and target nodes
            //Doesn't really matter "where" they are
            source = new NetworkNode("source", 0, 0);
            target = new NetworkNode("target", 0, 0);

            vertices[vertexCount] = source;
            vertices[vertexCount + 1] = target;

            Random rand = new Random(SEED);
            //List to keep track of the nodes that are conncected to by the source and target
            List<NetworkNode> sourceLinks = new List<NetworkNode>(K);
            List<NetworkNode> targetLinks = new List<NetworkNode>(K);
            //Index to keep track of where to add into the edges array
            int LinkIndex = 1;
            //generating the edges for source edges
            while(sourceLinks.Count < sourceLinks.Capacity)
            {
                int i = rand.Next(0, vertexCount);
                if (!sourceLinks.Contains(vertices[i]))
                {
                    sourceLinks.Add(vertices[i]);
                    edges[edgeCount - 1 + LinkIndex] = new NetworkLink(source, vertices[i]);
					edges[edgeCount - 1 + LinkIndex].capacity = 20;
                    LinkIndex++;
                }
            }
            //generating edges for target edges
            //Compares to make sure the target does not connect to any of the same nodes as the source.
            while(targetLinks.Count < targetLinks.Capacity)
            {
                int i = rand.Next(0, vertexCount);
                if (!sourceLinks.Contains(vertices[i]) && !targetLinks.Contains(vertices[i]))
                {
                    targetLinks.Add(vertices[i]);
                    edges[edgeCount - 1 + LinkIndex] = new NetworkLink(vertices[i], target);
					edges[edgeCount - 1 + LinkIndex].capacity = 20;
                    LinkIndex++;
                }
            }
			Console.WriteLine("Built graph from " + fileName + ". Nodes: " + vertexCount + " Links: " + edgeCount + ".");
        }

		// calculate the flow of the network from 'source' to 'target'.
		// this will set 'maxFlow' and will also increase 'flow' in the affected links.
		public void CalculateFlow()
		{
			Console.WriteLine("Beginning flow calculations.");
			
			// while the flow on the path is not zero,
			int flow;
			do
			{
				List<NetworkLink> path = GetMaxFlowPath(source, target, out flow);
				if(flow > 0)
				{
					Console.WriteLine("Found Path with flow of " + flow + ":");
					for(int i = 0; i < path.Count; i++)
					{
						Console.WriteLine("\t" + path[i].source.label);
						path[i].flow += flow;
					}
				}

			} while(flow != 0);

			// max flow is the amount of flow into the target node.
			maxFlow = target.GetFlow(false);

			Console.WriteLine("Flow calculations finished. Max flow: " + maxFlow);
		}
	
		// Finds the path that gets the maximum flow
		// This runs in O(2 * edges.Count) time. As edges get destroyed, it will run progressively faster.
		//		Edges with (flow == capacity) will not be assessed, if another edge connects to that edge's target node, the target will be assessed at that time.
		//		Nodes with no edges going into them will not be assessed, thus all of their exiting edges will not be assessed either.
		static List<NetworkLink> GetMaxFlowPath(NetworkNode sourceNode, NetworkNode finalNode, out int maxFlow)
		{
			Dictionary<NetworkNode, NetworkLink> flowPathTable = new Dictionary<NetworkNode, NetworkLink>();
			Dictionary<NetworkNode, int> flowAmountTable = new Dictionary<NetworkNode, int>();
			HashSet<NetworkNode> nodesAddedButNotRecorded = new HashSet<NetworkNode>();
			Stack<NetworkLink> cycleEdges = new Stack<NetworkLink>();

			// add the final node to the tables. This way, the algorithm will stop at the final node.
			flowPathTable.Add(finalNode, null);
			flowAmountTable.Add(finalNode, int.MaxValue);
			
			// recursively add nodes to the table, starting at the source node.
			AddNodeToTables(sourceNode, ref nodesAddedButNotRecorded, ref flowPathTable, ref flowAmountTable, ref cycleEdges);

			// reverse the stack in order to get the cycle links that are closest to the end.
			if(cycleEdges.Count > 0)
				cycleEdges = (Stack<NetworkLink>)cycleEdges.Reverse();

			// correct the table if necessary to incorporate the cycle edges.
			while(cycleEdges.Count > 0)
			{
				var edge = cycleEdges.Pop();

				// the new flow is the calculated using the cycle edge.
				int flow = Math.Min(flowAmountTable[edge.target], edge.capacity - edge.flow);

				// if the new flow is greater than the old one, use the new flow and the new direction.
				if(flow > flowAmountTable[edge.source])
				{
					flowPathTable[edge.source] = edge;
					flowAmountTable[edge.source] = flow;
				}
			}
			
			// get the maxFlow for the path
			maxFlow = flowAmountTable[sourceNode];

			// if there is 0 maxFlow, then there is no path to the target anymore.
			if(maxFlow == 0)
				return null;
			
			// build the path now that the tables are complete
			List<NetworkLink> path = new List<NetworkLink>();

			// set the first entry of the path to the source node's best choice edge.
			path.Add(flowPathTable[sourceNode]);

			// Add the most recent edge's target's best choice edge to the path.
			while(path[path.Count - 1].target != finalNode)
			{
				path.Add(flowPathTable[path[path.Count - 1].target]);
			}

			return path;
		}

		// adds the passed node to the tables recursively
		static void AddNodeToTables(
				NetworkNode node,
				ref HashSet<NetworkNode> nodesAddedButNotRecorded,
				ref Dictionary<NetworkNode, NetworkLink> flowPathTable,
				ref Dictionary<NetworkNode, int> flowAmountTable,
				ref Stack<NetworkLink> cycleEdges
			)
		{
			nodesAddedButNotRecorded.Add(node);

			NetworkLink bestChoice = null;
			int bestFlow = 0;

			// search the outward links for the link which offers the best path to the target.
			foreach(NetworkLink link in node.linksOut)
			{
				// if the link cannot pass anymore flow, skip it.
				if(link.capacity - link.flow == 0)
					continue;
				
				//	if the target of this link has been added already,
				//		and there is no entry for it in the flow path table,
				//		then this is a cycle edge. Don't evaluate it now.
				if(nodesAddedButNotRecorded.Contains(link.target))
				{
					cycleEdges.Push(link);
					continue;
				}
				else if(!flowPathTable.ContainsKey(link.target))
				{
					AddNodeToTables(link.target, ref nodesAddedButNotRecorded, ref flowPathTable, ref flowAmountTable, ref cycleEdges);
				}
				
				//	if the minimum between the possible increase in flow on this link
				//		and the possible increase in flow for the path to the target
				//		following this edge is greater than that of the other links
				//		investigated so far, select this one as the best choice.
				int flow = Math.Min(link.capacity - link.flow, flowAmountTable[link.target]);
				if(flow > bestFlow)
				{
					bestChoice = link;
					bestFlow = flow;
				}
			}

			//	add the best choice and best flow to the tables.
			//		if there were no outward links, or all of their (capacity - flow)s were 0, bestFlow will be 0.
			flowPathTable.Add(node, bestChoice);
			flowAmountTable.Add(node, bestFlow);

			nodesAddedButNotRecorded.Remove(node);
		}
	}
}
