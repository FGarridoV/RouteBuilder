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
    }
}
