using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RouteBuilder
{
    public class Network
    {
		//Class elements
        public List<Node> nodes;
		public List<Link> links;

        //Constructor from RealNetwork
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

        //Constructor to make copies
		public Network(Network net)
		{
			nodes = new List<Node>();
			links = new List<Link>();

			foreach (Node n in net.nodes)
			{
				Node nodeAux = new Node(n);
				nodes.Add(nodeAux);
			}

			foreach (Link l in net.links)
			{
				Link linkAux = new Link(l);
				int tn = l.tailNode.ID;
				int hn = l.headNode.ID;
				linkAux.add_tailNode(Node.pos_node_by_ID(tn, nodes));
				linkAux.add_headNode(Node.pos_node_by_ID(hn, nodes));
				Node.pos_node_by_ID(tn, nodes).add_outerLink(linkAux);
				Node.pos_node_by_ID(hn, nodes).add_innerLink(linkAux);
				links.Add(linkAux);
			}
		}

        //Method 1: Create a new network equals to the original
        public Network Clone()
        {
            Network n = new Network(this);
            return n;
        }

        //Method 2: Returns the node by a specific ID
        public Node nodeByID(int ID)
        {
            foreach(Node n in nodes)
            {
                if (n.ID == ID)
                    return n;
            }
            return null;
        }

        //Method 3: Returns the link by the specific nodeTailID and nodeHeadID
		public Link LinkByNodesID(int tailID, int headID)
		{
			foreach (Link l in links)
			{
                if (l.tailNode.ID == tailID && l.headNode.ID == headID)
					return l;
			}
			return null;
		}

        //Method 4: Determine the value of angular cost of a link 
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

        //Method 5: Assing the value of angular cost to all links 
        public void set_angularCosts(int SinkNode)
        {
            foreach(Link l in links)
            {
                l.angularCost = angular_cost_value(l,nodeByID(SinkNode));
            }
        }

        //Method 6: Indicates if it is possible go to one node to other in one step
        public bool Can_I_go_in_one_link(int nodeSource, int nodeSink)
        {
            if (LinkByNodesID(nodeSource, nodeSink) == null)
                return false;
            else
                return true;
        }

        //Method 7: Return a path of minimal cost from 2 nodes, returns null if it is impossible
        public Path dijkstra(int sourceNodeID, int sinkNodeID)
		{
            List<Node> allNodes = new List<Node>(nodes);
            List<Node> PermNodes = new List<Node>();

			foreach (Node n in allNodes)
			{
                if (n.ID == sourceNodeID)
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

            while (PermNodes.Count < nodes.Count)
			{
                int pos = minCost_pos(allNodes);
                PermNodes.Add(allNodes[pos]);

                if(allNodes[pos].ID == sinkNodeID)
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

        //Method 8: generate a path element from a node list output from Dijkstra
        public Path generate_path(List<Node> nList)
        {
            List<int> nods = new List<int>();
            nods.Add(nList[nList.Count - 1].ID);
            nods.Add(nList[nList.Count - 1].pTAG);
            nList.RemoveAt(nList.Count - 1);
            bool a = true;
            while (a)
            {
                int ver = 0;
                foreach (Node n in nList)
                {
                    if (n.ID == nods[nods.Count - 1])
                    {
                        ver=1;
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
                if (ver == 0)
                    return null;

            }
            nods.Reverse();
            Path ans = new Path(nods, this);
            return ans;
        }

        //Method 9: Indicates which node have the minCos cumulated to Dijkstra
		public int minCost_pos(List<Node> list)
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

        //Method 10: Assing the parameter mainCost to all links
        public void add_mainCost(int costType)
        {
			if (costType == 0)
			{
				foreach (Link l in this.links)
				{
					l.mainCost = l.distanceCost;
				}
			}

			else if (costType == 1)
			{
				foreach (Link l in this.links)
				{
					l.mainCost = l.edgesCost;
				}
			}

			else
			{
				foreach (Link l in this.links)
				{
					l.mainCost = l.angularCost;
				}
			}
        }        

        //Method 11: Determine Kht shortest path
        public List<Path> YenKsP(int sourceNodeID, int sinkNodeID, int K, int costType)
        {
            this.set_angularCosts(sinkNodeID);
            this.add_mainCost(costType);
            Network netAux = new Network(this);

            List<Path> A = new List<Path>();
            List<Path> B = new List<Path>();

            Path p0 = netAux.dijkstra(sourceNodeID, sinkNodeID);
            A.Add(p0);

            for (int k = 1; k <= K; k++)
            {
                for (int i = 0; i < A[k - 1].nodesIDs.Count - 1; i++)
                {
                    int spurNode = A[k - 1].nodesIDs[i];
                    List<int> rootPath = A[k - 1].some_nodes(0, i);

                    foreach (Path p in A)
                    {
                        if (Path.areEquals(rootPath, p.some_nodes(0, i)))
                        {
                            netAux.delete_link_by_nodes_id(p.nodesIDs[i], p.nodesIDs[i + 1]);
                        }
                    }

                    foreach (int rootNode in rootPath)
                    {
                        if (rootNode != spurNode)
                        {
                            netAux.delete_node_by_id(rootNode);
                        }
                    }

                    Path spurPath = netAux.dijkstra(spurNode, sinkNodeID);

                    if (spurPath == null)
                    {
                        netAux = new Network(this);
                        continue;
                    }

                    List<int> potentialNodes = new List<int>(rootPath);
                    potentialNodes.RemoveAt(potentialNodes.Count - 1);
                    foreach (int nod in spurPath.nodesIDs)
                    {
                        potentialNodes.Add(nod);
                    }

                    Path potential = new Path(potentialNodes, this);
                    B.Add(potential);
                    netAux = new Network(this);
                }

                if (B.Count == 0)
                    break;

                B.Sort();
                A.Add(B[0]);
                B.RemoveAt(0);
                for (int q = 0; q < B.Count;q++)
                {
                    if (Math.Abs(A[A.Count - 1].totalCost - B[q].totalCost) < 0.00001)
                    {
                        A.Add(B[q]);
                        B.RemoveAt(q);
                    }
                    else
                        break;
                }

                if(A.Count == K)
                {
                    return A;
                }
            }
            return A;
        }



        //Method A: Delete links from a network with specific IDS
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

        //Method B: Delete node by ID
		public void delete_node_by_id(int NodeID)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
                if (nodes[i].ID == NodeID)
				{
                    delete_links_that_contains_node(NodeID); 
					nodes.RemoveAt(i);
				}
			}
		}

        //Method C: Delete links that contains the specific node
        public void delete_links_that_contains_node(int NodeID)
        {
			for (int i = 0; i < links.Count; i++)
			{
                if (links[i].tailNode.ID == NodeID)
				{
                    nodeByID(links[i].headNode.ID).delete_innerLink(NodeID, links[i].headNode.ID);
					nodeByID(links[i].tailNode.ID).delete_outerLink(NodeID, links[i].headNode.ID);
					links.RemoveAt(i);
                    i--;
				}

				else if (links[i].headNode.ID == NodeID)
				{
                    nodeByID(links[i].headNode.ID).delete_innerLink(links[i].tailNode.ID, NodeID);
					nodeByID(links[i].tailNode.ID).delete_outerLink(links[i].tailNode.ID, NodeID);
					links.RemoveAt(i);
                    i--;
				}
			}  
        }

        public void export_data()
        {
            StreamWriter sw1 = new StreamWriter("dwellTimes.txt");
            StreamWriter sw2 = new StreamWriter("travelTimes.txt");

            foreach(Node n in nodes)
            {
                foreach(DwellTimes dt in n.dTimes)
                {
                    foreach(double t in dt.times)
                    {
                        sw1.WriteLine(n.ID + ", " + dt.ID_period + ", " + t);
                    }
                }
            }
            sw1.Close();

            foreach(Link l in links)
            {
                foreach (TravelTimes tt in l.tTimes)
				{
					foreach (double t in tt.times)
					{
						sw2.WriteLine(l.ID + ", " + tt.ID_period + ", " + t);
					}
				} 
            }
            sw2.Close();

        }
    }
}
