using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Network
    {
		public List<Node> nodes;
		public List<Link> links;

        public Network(RealNetwork rn)
        {
            nodes = new List<Node>();
            links = new List<Link>();

            foreach(RealNode n in rn.nodes)
            {
                Node nodeAux = new Node(n);
                nodes.Add(nodeAux);
            }

            foreach(RealLink l in rn.links)
            {
                Link linkAux = new Link(l);
                int tn = l.tailNode.ID;
                int hn = l.headNode.ID;
                linkAux.add_tailNode(Node.pos_node_by_ID(tn,nodes));
                linkAux.add_headNode(Node.pos_node_by_ID(hn, nodes));
                Node.pos_node_by_ID(tn,nodes).add_outerLink(linkAux);
                Node.pos_node_by_ID(hn, nodes).add_innerLink(linkAux);
                links.Add(linkAux);
            }
        }

        public Node nodeByID(int ID)
        {
            foreach(Node n in nodes)
            {
                if (n.ID == ID)
                    return n;
            }
            return null;
        }

		public Link LinkByNodesID(int tailID, int headID)
		{
			foreach (Link l in links)
			{
                if (l.tailNode.ID == tailID && l.headNode.ID == headID)
					return l;
			}
			return null;
		}

        public void set_angularCost_by_id(int linkID, double val)
        {
            foreach(Link l in links)
            {
                if(l.ID == linkID)
                {
                    l.angularCost = val;
                    break;
                }
            }
        }

        /*public List<Path> YenKsP(int SourceNodeID, int SinkNodeID, int k, int CostType)
        {
            
        }*/

    }
}
