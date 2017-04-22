using System;
using System.IO;

namespace Network_Project
{
	class NetworkGraph
	{
        public NetworkNode[] vertices; // IDs: 0 to 753
		public NetworkLink[] edges; // IDs: 0 to 898

		public NetworkNode source;
		public NetworkNode target;


		// pass the filename of the file containing the network info.
		public NetworkGraph(string fileName)
		{
            vertices = new NetworkNode[754];
            edges = new NetworkLink[899];
            // read file with the filename provided,
            // parse each vertex and edge into the graph.
            // each vertex and edge should be placed into the arrays at their id's index.

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
                                    Console.WriteLine("ID");
                                    string strID = arrNode[j].Substring(7);
                                    if (strID.Length > 1 && strID[1] == 'e')//added length check for id 0 of nodes
                                    {
                                        isEdge = true;
                                        id = Convert.ToInt32(strID.Substring(2, strID.Length - 3));
                                    }
                                    else
                                        id = Convert.ToInt32(strID);
                                    break;
                                case 'L':
                                    if(arrNode[j][5] == 'o')//Longitude
                                    {
                                        Console.WriteLine(arrNode[j].Substring(15));
                                        longitude = (float)Convert.ToDouble(arrNode[j].Substring(14));
                                    }
                                    else//Latitude
                                    {
                                        Console.WriteLine(arrNode[j].Substring(14));
                                        latitude = (float)Convert.ToDouble(arrNode[j].Substring(13));
                                    }
                                    break;
                                case 'l':
                                    Console.WriteLine(arrNode[j].Substring(11, arrNode[j].Length - 12));
                                    label = arrNode[j].Substring(11, arrNode[j].Length - 12);
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
                        }
                        else
                        {
                            NetworkNode node = new NetworkNode(label, longitude, latitude);
                            vertices[id] = node;
                        }
                    }
                }
            }
        }
	}
}
