using System.Collections.Generic;
using System;


namespace Network_Project
{
	class NetworkNode
	{
		public string label;
		public Coordinate pos;
		public List<NetworkLink> linksIn = new List<NetworkLink>();
		public List<NetworkLink> linksOut = new List<NetworkLink>();

		public NetworkNode(string label, float x, float y)
		{
			this.label = label;
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
	}
}
