using System;

namespace Network_Project
{
	class Program
	{
		const int SEED = 500;
        const string GRAPH_FILE = "Kdl.gml";
		const int MIN_CAPACITY = 1;
		const int MAX_CAPACITY = 20;

        static void Main(string[] args)
		{
			// build the graph using the file provided in the project description.
            NetworkGraph graph = new NetworkGraph(GRAPH_FILE);

			// randomly assign values to each edge's capacity between the min and max capacity.
			Random random = new Random(SEED);
			for(int i = 0; i < graph.edges.Length; i++)
			{
				graph.edges[i].capacity = random.Next(MIN_CAPACITY, MAX_CAPACITY + 1);
			}

			// set the graph's source and target nodes.
			graph.source = graph.vertices[0];
			graph.target = graph.vertices[559];

			// fill the graph's flow to achieve the max flow.
			graph.SetFlow();

			// pause so i can read the output.
			while(true) { }
		}
	}
}
