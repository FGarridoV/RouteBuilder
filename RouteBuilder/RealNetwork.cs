using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class RealNetwork
    {
        public List<RealNode> nodes;
        public List<RealLink> links;

        public RealNetwork(List<RealNode> nodes, List<RealLink> links)
        {
            this.nodes = new List<RealNode>(nodes);
            this.links = new List<RealLink>(links);
        }

        public RealNetwork(List<int[]> nodeInfo, List<int[]> LinkInfo)
        {
            this.nodes = new List<RealNode>();
            this.links = new List<RealLink>();

            foreach(int[] dat in nodeInfo)
            {
                RealNode aux = new RealNode(dat[0],dat[1],dat[2]);
                if(dat[3]==1)
                {
                    aux.set_sensor();
                }
                nodes.Add(aux);
            }

            foreach(int[] dat in LinkInfo)
            {
                RealLink aux = new RealLink(dat[0], nodes[dat[1]], nodes[dat[2]]);
                links.Add(aux);

                nodes[dat[1]].add_outerLink(aux);
                nodes[dat[2]].add_innerLink(aux);
            }
        }

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

    }
}
