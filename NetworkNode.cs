

namespace Network_Project
{
	class NetworkNode
	{
		public string label;
		public string state;
		public Coordinate pos;
		
		public NetworkNode(string label, string state, float x, float y)
		{
			this.label = label;
			this.state = state;
			pos = new Coordinate(x, y);
		}
	}
}
