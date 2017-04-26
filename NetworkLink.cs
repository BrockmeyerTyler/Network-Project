

namespace Network_Project
{
	class NetworkLink
	{
		public NetworkNode source;
		public NetworkNode target;
		public int capacity = 0;
		public int flow = 0;

		public NetworkLink(NetworkNode s, NetworkNode t)
		{
			source = s; 
			target = t;

			source.AddLink(this);
			target.AddLink(this);
		}

		public override string ToString()
		{
			return source.ToString() + " ----- " + target.ToString() + " | Cap: " + capacity + " Flow: " + flow;
		}
	}
}
