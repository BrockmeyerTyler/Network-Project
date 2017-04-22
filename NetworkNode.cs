using System.Collections.Generic;
using System;


namespace Network_Project
{
	class NetworkNode
	{
		public string label;
		public string state;
		public Coordinate pos;
		public List<NetworkLink> linksIn = new List<NetworkLink>();
		public List<NetworkLink> linksOut = new List<NetworkLink>();
		
		public NetworkNode(string label, string state, float x, float y)
		{
			this.label = label;
			this.state = state;
			pos = new Coordinate(x, y);
		}

		// adds the network link to the correct link list.
		public void AddLink(NetworkLink link)
		{
			(link.source == this ? linksOut : linksIn).Add(link);
		}

		// gets the total capacity going in or out of this node
		public int GetCapacity(bool _out)
		{
			int capacity = 0;
			foreach(NetworkLink link in (_out ? linksOut : linksIn))
			{
				capacity += link.capacity;
			}
			return capacity;
		}

		// gets the total flow going in or out of this node
		public int GetFlow(bool _out)
		{
			int flow = 0;
			foreach(NetworkLink link in (_out ? linksOut : linksIn))
			{
				flow += link.flow;
			}
			return flow;
		}

		// finds the path that gets the maximum flow
		public static List<NetworkLink> GetMaxFlowPath(NetworkNode sourceNode, NetworkNode finalNode, out int maxFlow)
		{
			Dictionary<NetworkNode, NetworkLink> flowPathTable = new Dictionary<NetworkNode, NetworkLink>();
			Dictionary<NetworkNode, int> flowAmountTable = new Dictionary<NetworkNode, int>();

			// add the final node to the tables. This way, the algorithm will stop at the final node.
			flowPathTable.Add(finalNode, null);
			flowAmountTable.Add(finalNode, int.MaxValue);
			
			// recursively add nodes to the table, starting at the source node.
			AddNodeToTables(sourceNode, ref flowPathTable, ref flowAmountTable);
			
			// build the path now that the tables are complete
			List<NetworkLink> path = new List<NetworkLink>();
			maxFlow = flowAmountTable[sourceNode];

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
		static void AddNodeToTables(NetworkNode node, ref Dictionary<NetworkNode, NetworkLink> flowPathTable, ref Dictionary<NetworkNode, int> flowAmountTable)
		{
			if(flowPathTable.ContainsKey(node))
				return;

			NetworkLink bestChoice = null;
			int bestFlow = 0;

			// search the outward links for the link which offers the best path to the target.
			foreach(NetworkLink link in node.linksOut)
			{
				if(!flowPathTable.ContainsKey(link.target))
					AddNodeToTables(link.target, ref flowPathTable, ref flowAmountTable);
				
				int flow = Math.Min(link.capacity - link.flow, flowAmountTable[link.target]);
				if(flow > bestFlow)
				{
					bestChoice = link;
					bestFlow = flow;
				}
			}

			flowPathTable.Add(node, bestChoice);
			flowAmountTable.Add(node, bestFlow);
		}
	}
}
