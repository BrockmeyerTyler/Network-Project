

namespace Network_Project
{
	class NetworkLink
	{
		public NetworkNode source;
		public NetworkNode target;

		public NetworkLink(NetworkNode s, NetworkNode t)
		{
			source = s; 
			target = t;
		}
	}
}
