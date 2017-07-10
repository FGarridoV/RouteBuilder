using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class RealNetwork
    {
        //Class elements
        public List<RealNode> nodes;
        public List<RealLink> links;

        //Constructor to create a RealNetwork
        public RealNetwork(List<double[]> nodeInfo, List<double[]> LinkInfo)
        {
            this.nodes = new List<RealNode>();
            this.links = new List<RealLink>();

            foreach(double[] dat in nodeInfo)
            {
                RealNode aux = new RealNode((int) dat[0],dat[1],dat[2]);
                if((int)dat[3]==1)
                {
                    aux.set_sensor();
                }
                nodes.Add(aux);
            }

            foreach(double[] dat in LinkInfo)
            {
                RealLink aux = new RealLink((int)dat[0], RealNode.node_by_ID(nodes,(int)dat[1]), RealNode.node_by_ID(nodes, (int)dat[2]));
                links.Add(aux);

                RealNode.node_by_ID(nodes, (int)dat[1]).add_outerLink(aux);
                RealNode.node_by_ID(nodes, (int)dat[2]).add_innerLink(aux);
            }
        }

		//Constructor to tranform real to model
		public RealNetwork(List<RealNode> nodes, List<RealLink> links)
		{
			this.nodes = new List<RealNode>(nodes);
			this.links = new List<RealLink>(links);
		}

        //Method 1: Create a model network in a real interface
        public RealNetwork real_to_model()
        {
            List<RealNode> newNodes = new List<RealNode>();
            List<RealLink> newLinks = new List<RealLink>();

            for (int i = 0; i < this.nodes.Count;i++)
            {
                if(this.nodes[i].hasSensor)
                {
                    RealNode nodeAux = new RealNode(nodes[i]);
                    newNodes.Add(nodeAux);
                }
            }
            int k = 0;
            foreach (RealNode n in newNodes)
            {
                List<int> auxNodes = new List<int>(search_new_links(RealNode.node_by_ID(nodes,n.ID)));

                foreach(int num in auxNodes)
                {
                    RealLink rl = new RealLink(k, n, RealNode.node_by_ID(newNodes,num));
                    n.add_outerLink(rl);
                    RealNode.node_by_ID(newNodes, num).add_innerLink(rl);
                    newLinks.Add(rl);
                    k++;
                }
            }
            RealNetwork model = new RealNetwork(newNodes, newLinks);
            return model;
        }

        //Method 2: Generate the new links
        public List<int> search_new_links(RealNode n)
        {
            List<int> headNodes = new List<int>();

            foreach (RealLink l in n.outerLinks)
            {
                if(!l.headNode.hasSensor)
                {
                    headNodes.AddRange(search_new_links(l.headNode));
                }
                else
                {
                    headNodes.Add(l.headNode.ID);
                }
            }

            return headNodes;
        }

        //Method 3: Determinate de minimal distance 
        public double dijkstraDist(int nodeID1, int nodeID2)
        {
            List<RealNode> DijkNodes = new List<RealNode>(nodes);
            double distance = 0;

            foreach(RealNode n in DijkNodes)
            {
                if (n.ID == nodeID1)
                    n.set_dijkstraTag(0);
                else
                    n.set_dijkstraTag(double.PositiveInfinity);
            }

            while (DijkNodes.Count>0)
            {
                
                int pos = minPos(DijkNodes);
                RealNode permanent = DijkNodes[pos];

				if (DijkNodes[pos].ID == nodeID2)
					distance = permanent.get_dijkstraTag();

                DijkNodes.RemoveAt(pos);

                foreach(RealLink l in permanent.outerLinks)
                {
                    if (l.headNode.get_dijkstraTag() > permanent.get_dijkstraTag() + l.distance)
                        l.headNode.set_dijkstraTag(permanent.get_dijkstraTag() + l.distance);
                }
            }

            return distance;

        }

        //Method 4: Determinate the minamal number of nodes
		public double dijkstraNodes(int nodeID1, int nodeID2)
		{
			List<RealNode> DijkNodes = new List<RealNode>(nodes);
			double distance = 0;

			foreach (RealNode n in DijkNodes)
			{
				if (n.ID == nodeID1)
					n.set_dijkstraTag(0);
				else
					n.set_dijkstraTag(double.PositiveInfinity);
			}

			while (DijkNodes.Count > 0)
			{

				int pos = minPos(DijkNodes);
				RealNode permanent = DijkNodes[pos];

				if (DijkNodes[pos].ID == nodeID2)
					distance = permanent.get_dijkstraTag();

				DijkNodes.RemoveAt(pos);

				foreach (RealLink l in permanent.outerLinks)
				{
					if (l.headNode.get_dijkstraTag() > permanent.get_dijkstraTag() + 1)
						l.headNode.set_dijkstraTag(permanent.get_dijkstraTag() + 1);
				}
			}

			return distance;

		}

        //Method 5: calculate the position of the minimal node
        public int minPos(List<RealNode> list)
        {
            double min = list[0].get_dijkstraTag();
            int resp = -1;
            for (int i = 0; i < list.Count;i++)
            {
                if (list[i].get_dijkstraTag() <= min)
                {
                    min = list[i].get_dijkstraTag();
                    resp = i;
                }   
            }
            return resp;
        }

        //Method 6: Set data of dijkstras
        public void set_DijkstraData(RealNetwork RealNet)
        {
            foreach(RealLink l in this.links)
            {
                double nod = RealNet.dijkstraNodes(l.tailNode.ID, l.headNode.ID);
                double dist = RealNet.dijkstraDist(l.tailNode.ID, l.headNode.ID);
                l.set_dijkstraDistance(dist);
                l.set_dijkstraNodes(nod);
            }
        }

        public List<int> BTS_id()
        {
            List<int> BTS = new List<int>();
            foreach(RealNode n in nodes)
            {
                if(n.hasSensor)
                {
                    BTS.Add(n.ID);
                }
            }
            return BTS;
        }
    }
}
