using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Node
    {
		public int ID;
		public List<Link> innerLinks;
		public List<Link> outerLinks;
		public double x;
        public double y;
        public List<DwellTimes> dTimes;

        public Node(RealNode n)
        {
            this.ID = n.ID;
            this.innerLinks = new List<Link>();
            this.outerLinks = new List<Link>();
            this.x = n.get_position()[0];
            this.y = n.get_position()[1];
            this.dTimes = new List<DwellTimes>();
        }

        public void add_innerLink(Link l)
        {
            innerLinks.Add(l);
        }

		public void add_outerLink(Link l)
		{
			outerLinks.Add(l);
		}


        public static Node pos_node_by_ID(int ID, List<Node>nodeList)
        {
            int i = 0;
            int resp = 0;
            foreach (Node n in nodeList)
            {
                if (n.ID == ID)
                {
                    resp = i;
                }
                i++;
            }
            return nodeList[resp];
        }

        public void set_dwell_time_at_period(int period, double time)
        {
            if (exist_period(period))
            {
                foreach(DwellTimes d in dTimes)
                {
                    if (d.ID_period == period)
                        d.add_dTime(time);
                }
            }
            else
            {
                DwellTimes dts = new DwellTimes(period);
                dts.add_dTime(time);
                dTimes.Add(dts);
            }
            
        }

        public bool exist_period(int period)
        {
            foreach(DwellTimes d in dTimes)
            {
                if (d.ID_period == period)
                    return true;
            }

            return false;
        }
    }
}
