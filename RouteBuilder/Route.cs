using System;
using System.Collections.Generic;
namespace RouteBuilder
{
    public class Route
    {
		int MAC;
		public List<int> nodes;
		public double prob;

        public Route(int MAC, List<int> nodes, double prob)
        {
            this.MAC = MAC;
            this.prob = prob;
            this.nodes = new List<int>(nodes); 
        }

        public string export_route()
        {
            string text = "";
            int j = 0;
            foreach(int i in nodes)
            {
                if(j==nodes.Count-1)
                    text += i + " // Prob: " + prob;

                else
                {
					text += i + " > ";
                }
           
                j++;
            }

            return text;
        }
        /////BORRAR///
        public static bool Comparer(Route r1, Route r2)
        {
            if(r1.nodes.Count==r2.nodes.Count)
            {
                for (int i = 0; i < r1.nodes.Count;i++)
                {
                    if (r1.nodes[i] == r2.nodes[i])
                        continue;
                    else
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
