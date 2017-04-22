

namespace Network_Project
{
	class NetworkGraph
	{
		public NetworkNode[] vertices = new NetworkNode[754]; // IDs: 0 to 753
		public NetworkLink[] edges = new NetworkLink[899]; // IDs: 0 to 898

		public NetworkNode source;
		public NetworkNode target;


		// pass the filename of the file containing the network info.
		public NetworkGraph(string fileName)
		{
			// read file with the filename provided,
			// parse each vertex and edge into the graph.
			// each vertex and edge should be placed into the arrays at their id's index.
		}
	}
}
