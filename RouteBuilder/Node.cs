using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Node
    {
        //Class elements
        public int ID;
        public List<Link> innerLinks;
        public List<Link> outerLinks;
        public double x;
        public double y;
        public List<DwellTimes> dTimes;
        public double cTAG;
        public int pTAG;

        //Constructor from RealNode
        public Node(RealNode n)
        {
            this.ID = n.ID;
            this.innerLinks = new List<Link>();
            this.outerLinks = new List<Link>();
            this.x = n.get_position()[0];
            this.y = n.get_position()[1];
            this.dTimes = new List<DwellTimes>();
        }

        //Constructor to make copies
        public Node(Node n)
        {
            this.ID = n.ID;
            this.x = n.x;
            this.y = n.y;
            this.cTAG = n.cTAG;
            this.pTAG = n.pTAG;
            this.dTimes = new List<DwellTimes>();
            this.innerLinks = new List<Link>();
            this.outerLinks = new List<Link>();
        }

        //Method 1: Add a innerlink to node 
        public void add_innerLink(Link l)
        {
            innerLinks.Add(l);
        }

        //Method 2: Add a outerlink to node
        public void add_outerLink(Link l)
        {
            outerLinks.Add(l);
        }

        //Method 3: Returns the node in the specified position
        public static Node pos_node_by_ID(int ID, List<Node> nodeList)
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

        //Method 4: Set a time at the period specified
        public void set_dwell_time_at_period(int period, double time)
        {
            if (exist_period(period))
            {
                foreach (DwellTimes d in dTimes)
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

        //Method 5: Indicates if the periods exist
        public bool exist_period(int period)
        {
            foreach (DwellTimes d in dTimes)
            {
                if (d.ID_period == period)
                    return true;
            }

            return false;
        }

		//Method 5: Return the times of specific period
		public DwellTimes dt_at_specific_period(int period)
		{
			foreach (DwellTimes dt in dTimes)
			{
                if (dt.ID_period == period)
                    return dt;
			}
            return null;
		}

		//Method A: Delete the innerlink with the ID
		public void delete_innerLink(int tailNodeID, int headNodeID)
		{
            for (int i = 0; i < innerLinks.Count; i++)
			{
				if (innerLinks[i].tailNode.ID == tailNodeID && innerLinks[i].headNode.ID == headNodeID)
				{
                    innerLinks.RemoveAt(i);
				}
			}
		}

        //Method B: Delete the outerlink with the ID
		public void delete_outerLink(int tailNodeID, int headNodeID)
		{
			for (int i = 0; i < outerLinks.Count; i++)
			{
				if (outerLinks[i].tailNode.ID == tailNodeID && outerLinks[i].headNode.ID == headNodeID)
				{
					outerLinks.RemoveAt(i);
				}
			}
		}
	}

}
