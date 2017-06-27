using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

		public double angular_cost_value(Link l, Node sink)
		{
			double[] p1 = new double[] { l.tailNode.x, l.tailNode.y };
			double[] p2 = new double[] { l.headNode.x, l.headNode.y };
			double[] p3 = new double[] { sink.x, sink.y };

			double a = Math.Sqrt(Math.Pow(p2[1] - p1[1], 2) + Math.Pow(p2[0] - p1[0], 2));
			double b = Math.Sqrt(Math.Pow(p3[1] - p1[1], 2) + Math.Pow(p3[0] - p1[0], 2));
			double c = Math.Sqrt(Math.Pow(p2[1] - p3[1], 2) + Math.Pow(p2[0] - p3[0], 2));

			double cosAngle = (Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b);
			double alfa = Math.Acos(cosAngle);

			double Acost = l.distanceCost * Math.Sin(alfa / 2);
			return Acost;
		}

        public void set_angularCosts(int SinkNode)
        {
            foreach(Link l in links)
            {
                l.angularCost = angular_cost_value(l,nodeByID(SinkNode));
            }
        }

        public bool Can_I_go_in_one_link(int nodeSource, int nodeSink)
        {
            if (LinkByNodesID(nodeSource, nodeSink) == null)
                return false;
            else
                return true;
        }

        public Path dijkstra(int SourceNodeID, int SinkNodeID, Network net)
		{
			List<Node> allNodes = new List<Node>(net.nodes);
            List<Node> PermNodes = new List<Node>();

			foreach (Node n in allNodes)
			{
                if (n.ID == SourceNodeID)
                {
                    n.cTAG = 0;
                    n.pTAG = -1;
                }

                else
                {
                    n.cTAG = double.PositiveInfinity;
                    n.pTAG = new int();
                }
			}

            while (PermNodes.Count < net.nodes.Count)
			{
                int pos = minPos(allNodes);
                PermNodes.Add(allNodes[pos]);

                if(allNodes[pos].ID == SinkNodeID)
                {
                    break;
                }

                allNodes.RemoveAt(pos);

                foreach(Link l in PermNodes[PermNodes.Count-1].outerLinks)
                {
                    double AcumlateCost = PermNodes[PermNodes.Count - 1].cTAG;
                    double newCost = l.mainCost;

                    if (l.headNode.cTAG > AcumlateCost + newCost)
                    {
                        l.headNode.cTAG = AcumlateCost + newCost;
                        l.headNode.pTAG = PermNodes[PermNodes.Count - 1].ID;
                    }
                }
			}

            return generate_path(PermNodes);

		}

        public Path generate_path(List<Node> nList)
        {
            List<int> nods = new List<int>();
            nods.Add(nList[nList.Count - 1].ID);
            nods.Add(nList[nList.Count - 1].pTAG);
            nList.RemoveAt(nList.Count - 1);
            bool a = true;
            while (a)
            {
                foreach (Node n in nList)
                {
                    if (n.ID == nods[nods.Count - 1])
                    {
                        if(n.pTAG==-1)
                        {
                            a = false;
                            break;
                        }
                        nods.Add(n.pTAG);
                        nList.Remove(n);
                        break;
                    }
                }
            }
            nods.Reverse();
            Path ans = new Path(nods, this);
            return ans;
        }

		public int minPos(List<Node> list)
		{
			double min = list[0].cTAG;
			int resp = -1;
			for (int i = 0; i < list.Count; i++)
			{
                if (list[i].cTAG <= min)
				{
					min = list[i].cTAG;
					resp = i;
				}
			}
			return resp;
		}

        public List<Path> YenKsP(int SourceNodeID, int SinkNodeID, int K, int CostType, RealNetwork mn)
        {
            Network netAux = new Network(mn);
            netAux.set_angularCosts(SinkNodeID);

            if (CostType == 0)
            {
                foreach(Link l in netAux.links)
                {
                    l.mainCost = l.distanceCost;
                }
            }

            else if (CostType == 1)
            {
				foreach (Link l in netAux.links)
				{
                    l.mainCost = l.edgesCost;
				}
            }

            else
            {
				foreach (Link l in netAux.links)
				{
                    l.mainCost = l.angularCost;
				}
            }

            List<Path> A = new List<Path>();
            List<Path> B = new List<Path>();

            Path p0 = dijkstra(SourceNodeID, SinkNodeID, netAux);
            A.Add(p0);

            for (int k = 1; k <= K;k++)
            {
                for (int i = 0; i < A[k - 1].nodesIDs.Count;i++)
                {
                    int spurNode = A[k - 1].nodesIDs[i];
                    List<int> rootPath = A[k - 1].some_nodes(0, i);

                    foreach(Path p in A)
                    {
                        if(Path.areEquals(rootPath,p.some_nodes(0,i)))
                        {
                            netAux.delete_link_by_nodes_id(p.nodesIDs[i],p.nodesIDs[i+1]);
                        }
                    }

                    foreach (int rootNode in rootPath)
                    {
                        if(rootNode != spurNode)
                        {
                            //remover rootNode del grafo
                        }
                    }

                    Path spurPath = dijkstra(spurNode, SinkNodeID, netAux);
                    List<int> potentialNodes = new List<int>(rootPath);
                    potentialNodes.RemoveAt(potentialNodes.Count-1);
                    foreach(int nod in spurPath.nodesIDs)
                    {
                        potentialNodes.Add(nod);
                    }

                    Path potential = new Path(potentialNodes, this);
                    B.Add(potential);
                    netAux = new Network(mn);
                }

                if (B.Count == 0)
                    break;
                
            }


            return A;
            
        }

        //methods just for clones of Network

        public void delete_link_by_nodes_id(int tailNodeID, int headNodeID)
        {
            for (int i = 0; i < links.Count;i++)
            {
                if (links[i].tailNode.ID == tailNodeID && links[i].headNode.ID == headNodeID)
                {
                    nodeByID(links[i].headNode.ID).delete_innerLink(tailNodeID, headNodeID);
                    nodeByID(links[i].tailNode.ID).delete_outerLink(tailNodeID, headNodeID);
                    links.RemoveAt(i);
                }
            }
        }

    }
}
