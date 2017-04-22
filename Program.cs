using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Project
{
	class Program
	{
        static string file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\visual studio 2015\Projects\Network-Project\Network-Project\Kdl.gml";


        static void Main(string[] args)
		{
            NetworkGraph graph = new NetworkGraph(file);
            Console.ReadLine();
		}
	}
}
