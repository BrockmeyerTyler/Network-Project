

namespace Network_Project
{
	class NetworkNode
	{
		public string label;
		public Coordinate pos;

		public NetworkNode(string label, float x, float y)
		{
			this.label = label;
			pos = new Coordinate(x, y);
		}
	}
}
