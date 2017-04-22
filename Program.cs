using System;

namespace Network_Project
{
	class Program
	{
        // static string file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\visual studio 2015\Projects\Network-Project\Network-Project\Kdl.gml";
		static string fileName = "Kdl.gml";

        static void Main(string[] args)
		{
            NetworkGraph graph = new NetworkGraph(fileName);
		}
	}
}
