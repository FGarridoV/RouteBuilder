using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Path : IComparable<Path>
    {
		//Class elements
        public List<int> nodesIDs;
		public List<Node> nodes;
		public List<Link> links;
        public double totalCost;

        //Constructor
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

            add_totalCost();
        }

        //Method 1: Compare 2 paths by nodes ID
        public bool Equals(Path p)
        {
            if(this.nodesIDs.Count == p.nodesIDs.Count)
            {
                for (int i = 0; i < this.nodesIDs.Count;i++)
                {
                    if (this.nodesIDs[i] == p.nodesIDs[i])
                        continue;
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        //Method 2: Compare 2 list of ints
        public static bool areEquals(List<int> nodes1,List<int> nodes2) 
        {
			if (nodes1.Count == nodes2.Count)
			{
				for (int i = 0; i < nodes1.Count; i++)
				{
					if (nodes1[i] == nodes2[i])
						continue;
					else
						return false;
				}
			}

			else
			{
				return false;
			}
			return true;
        }

        //Method 3: Return a subgroup of nodes ints id
        public List<int> some_nodes(int start_index, int last_index)
        {
            List<int> someNodes = new List<int>();
            for (int i = start_index; i <= last_index;i++)
            {
                someNodes.Add(nodesIDs[i]);
            }
            return someNodes;
        }

        //Method 4: Add the total cost to the Path
        public void add_totalCost()
        {
            foreach(Link l in links)
            {
                this.totalCost += l.mainCost;
            }
        }

        //Method 5: Compare to path with the cost
        public int CompareTo(Path other)
        {
            if (this.totalCost > other.totalCost)
				return 1;

            else if (this.totalCost < other.totalCost)
				return -1;

			else
				return 0;
        }
    }
}
