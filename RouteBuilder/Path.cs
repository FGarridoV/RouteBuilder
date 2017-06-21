using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Path
    {
		public List<int> nodesIDs;
		public List<Node> nodes;
		public List<Link> links;

        public Path(List<int> nodesIDs, Network net)
        {
            this.nodesIDs = new List<int>(nodesIDs);
            nodes = new List<Node>();
            links = new List<Link>();

            for (int i = 0; i < nodesIDs.Count; i++)
            {
                nodes.Add(net.nodeByID(nodesIDs[i]));

                if (i < nodesIDs.Count - 1)
                    links.Add(net.LinkByNodesID(nodesIDs[i], nodesIDs[i + 1]));
            }
        }

        public void set_angular_costs(Network net)
        {
            double angCost;

            foreach(Link l in links)
            {
                angCost = angular_cost_value(l, nodes[nodes.Count - 1]);
                net.set_angularCost_by_id(l.ID,angCost);
            }
        }

        public double angular_cost_value(Link l, Node sink)
        {
            double[] p1 = new double[] { l.tailNode.x , l.tailNode.y };
            double[] p2 = new double[] { l.headNode.x, l.headNode.y };
            double[] p3 = new double[] { sink.x, sink.y };

            double a = Math.Sqrt(Math.Pow(p2[1] - p1[1], 2) + Math.Pow(p2[0] - p1[0], 2));
            double b = Math.Sqrt(Math.Pow(p3[1] - p1[1], 2) + Math.Pow(p3[0] - p1[0], 2));
            double c = Math.Sqrt(Math.Pow(p2[1] - p3[1], 2) + Math.Pow(p2[0] - p3[0], 2));

            double cosAngle = (Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b);
            double alfa = Math.Acos(cosAngle);

            double Acost = l.distanceCost * Math.Sin(alfa / 2);
            return Acost;
        }

       
    }
}
