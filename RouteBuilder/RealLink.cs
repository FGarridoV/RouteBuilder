using System;

namespace RouteBuilder
{
    public class RealLink
    {
        //Class elements
        public int ID;
        public RealNode tailNode;
        public RealNode headNode;
        public double distance;
        public double auxDistance;
        public double dijkstraDistance;
        public double dijkstraNodes;

        //Constructor
        public RealLink(int ID, RealNode tailNode, RealNode headNode)
        {
            this.ID = ID;
            this.tailNode = tailNode;
            this.headNode = headNode;

			double x1 = tailNode.get_position()[0];
			double y1 = tailNode.get_position()[1];
			double x2 = headNode.get_position()[0];
			double y2 = headNode.get_position()[1];

			this.distance = Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2));
        }

        //Method 1: Add the dijkstra distance
        public void set_dijkstraDistance(double distance)
        {
            this.dijkstraDistance = distance;
        }

        //Method 2: Add the dijkstra nodes
		public void set_dijkstraNodes(double nodes)
		{
			this.dijkstraNodes = nodes;
		}
    }

}
