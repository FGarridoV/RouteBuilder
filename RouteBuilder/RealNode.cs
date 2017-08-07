using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class RealNode
    {
        //Class elements
        public int ID;
        public List<RealLink> innerLinks;
        public List<RealLink> outerLinks;
        double[] position;
        public bool hasSensor;
        double[] dijkstraTag;

        //Constructor with info
        public RealNode(int ID, double posX, double posY)
        {
            this.ID = ID;
            this.innerLinks = new List<RealLink>();
            this.outerLinks = new List<RealLink>();
            this.position = new double[] { posX, posY };
            hasSensor = false;
            dijkstraTag = new double[2];
        }

        //Constructor with another node
        public RealNode(RealNode node)
        {
            this.ID = node.ID;
            this.innerLinks = new List<RealLink>();
            this.outerLinks = new List<RealLink>();
            this.position = new double[] { node.get_position()[0], node.get_position()[1] };
            this.hasSensor = node.hasSensor;
        }

        //Method 1: set a sensor to node 
        public void set_sensor()
        {
            this.hasSensor = true;
        }

        //Method 2: add an innerlink
        public void add_innerLink(RealLink l)
        {
            innerLinks.Add(l);
        }

		//Method 3: add an outerlink
		public void add_outerLink(RealLink l)
		{
			outerLinks.Add(l);
		}

		//Method 4: returns the position
		public double[] get_position()
        {
            return position;
        }

		//Method 5: return a node by ID
		public static RealNode node_by_ID(List<RealNode> list, int ID)
        {
            foreach(RealNode n in list)
            {
                if (n.ID == ID)
                    return n;
            }
            return null;
        }

		//Method 6: assing a dijkstra tag
		public void set_dijkstraTag(double distance, int pos)
        {
            this.dijkstraTag[pos] = distance;      
        }

		//Method 7: obtain a dijkstra tag
		public double[] get_dijkstraTag()
        {
            return this.dijkstraTag;
        }
    }

}
