using System;

namespace Network_Project
{
	class Program
	{
        const string GRAPH_FILE = "Kdl.gml";
		const int VERTEX_COUNT = 754;
		const int EDGE_COUNT = 899;
		const int MIN_CAPACITY = 1;
		const int MAX_CAPACITY = 20;
		const int SEED = 4;

        static void Main(string[] args)
		{
			// the number of links generated from the source node and to the target node
			int K = 20;

			// build the graph using the file provided in the project description.
            NetworkGraph graph = new NetworkGraph(GRAPH_FILE, VERTEX_COUNT, EDGE_COUNT, MIN_CAPACITY, MAX_CAPACITY, K, SEED);

			// fill the graph's flow to achieve the max flow.
			graph.CalculateFlow();

            // pause so i can read the output.
            //It will pause the console until a return is given, I find it easier to control than an endless while loop
            Console.ReadLine();
        }
    }
}
