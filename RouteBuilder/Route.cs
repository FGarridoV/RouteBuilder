using System;
using System.Collections.Generic;
namespace RouteBuilder
{
    public class Route
    {
		int MAC;
		List<int> nodes;
		double prob;

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
    }
}
