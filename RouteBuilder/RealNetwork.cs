using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class RealNetwork
    {
        List<RealNode> nodes;
        List<RealLink> links;

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

        /*public double[] dijkstra(int nodeID1, int nodeID2)
        {


        }*/
    }
}
