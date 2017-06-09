using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class RealNode
    {
        public int ID;
        public List<RealLink> innerLinks;
        public List<RealLink> outerLinks;
        double[] position;
        public bool hasSensor;
        double dijkstraTag;

        public RealNode(int ID, double posX, double posY)
        {
            this.ID = ID;
            this.innerLinks = new List<RealLink>();
            this.outerLinks = new List<RealLink>();
            this.position = new double[] { posX, posY };
            hasSensor = false;
        }

        public RealNode(RealNode node)
        {
            this.ID = node.ID;
            this.innerLinks = new List<RealLink>();
            this.outerLinks = new List<RealLink>();
            this.position = new double[] { node.get_position()[0], node.get_position()[1] };
            this.hasSensor = node.hasSensor;
        }

        public void set_sensor()
        {
            this.hasSensor = true;
        }

        public void add_innerLink(RealLink l)
        {
            innerLinks.Add(l);
        }

		public void add_outerLink(RealLink l)
		{
			outerLinks.Add(l);
		}

        public double[] get_position()
        {
            return position;
        }

        public static RealNode node_by_ID(List<RealNode> list, int ID)
        {
            foreach(RealNode n in list)
            {
                if (n.ID == ID)
                    return n;
            }
            return null;
        }

        public void set_dijkstraTag(double distance)
        {
            this.dijkstraTag = distance;      
        }

        public double get_dijkstraTag()
        {
            return this.dijkstraTag;
        }
    }
}
