using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Trip
    {
        //Class elements
        public List<Detection> detections;
        public List<int> passingNodes;
        public List<double> enterTimePassingNodes;
        public List<double> exitTimePassingNodes;
        public List<double> countsOnPassingNodes;
        public List<Section> sections;
        public List<Route> routes;

        //Constructor
        public Trip()
        {
            detections = new List<Detection>();
            passingNodes = new List<int>();
            enterTimePassingNodes = new List<double>();
            exitTimePassingNodes = new List<double>(); 
            sections = new List<Section>();
            countsOnPassingNodes = new List<double>();
            routes = new List<Route>();
        }

        //Method 1: Add a detection
        public void add_detection(Detection d)
        {
            detections.Add(d);
        }

        //Method 2: Obtain the time of the last detection
        public double get_last_time()
        {
            return detections[detections.Count - 1].time;
        }

        //Method 3: Generate the passing nodes
        public void generate_passingNodes()
        {
            passingNodes.Add(detections[0].BSID);
            enterTimePassingNodes.Add(detections[0].time);
            exitTimePassingNodes.Add(detections[0].time);
            double aux = 1;
            countsOnPassingNodes.Add(aux);

            for (int i = 1; i < detections.Count;i++)
            {
                if(passingNodes[passingNodes.Count-1]==detections[i].BSID)
                {
                    countsOnPassingNodes[countsOnPassingNodes.Count - 1]++;
                    exitTimePassingNodes[exitTimePassingNodes.Count - 1] = detections[i].time;
                    continue;
                }
                else
                {
                    passingNodes.Add(detections[i].BSID);
                    enterTimePassingNodes.Add(detections[i].time);
                    exitTimePassingNodes.Add(detections[i].time);
                    double auxN = 1;
                    countsOnPassingNodes.Add(auxN);
                }
            }
        }

        public int sCounts(double time)
        {
            for (int i = 0; i < exitTimePassingNodes.Count;i++)
            {
                if(Math.Abs(time - exitTimePassingNodes[i]) < 0.00001)
                {
                    return (int)countsOnPassingNodes[i];
                }
            }
            return 0;
        }

		public int eCounts(double time)
		{
			for (int i = 0; i < enterTimePassingNodes.Count; i++)
			{
				if (Math.Abs(time - enterTimePassingNodes[i]) < 0.00001)
				{
					return (int)countsOnPassingNodes[i];
				}
			}
			return 0;
		}

        //Method 4: Add de sections of paths 
        public void add_sections(Network net, int k, double T)
        {
            for (int i = 0; i < detections.Count - 1; i++)
			{
                if (detections[i].BSID==detections[i+1].BSID)
				{
					continue;
				}

				else
				{
                    if (net.Can_I_go_in_one_link(detections[i].BSID,detections[i+1].BSID))
					{
                        Section opt = new Section(net, detections[i].BSID, detections[i + 1].BSID, detections[i].time, detections[i + 1].time, T,sCounts(detections[i].time), eCounts(detections[i+1].time));
                        sections.Add(opt);
					}

					else
					{
                        Section opt = new Section(net, detections[i].BSID, detections[i + 1].BSID,k,detections[i].time, detections[i + 1].time, T, sCounts(detections[i].time), eCounts(detections[i+1].time));
						sections.Add(opt);
					}
				}
			}
        }

        public void create_Routes(int MAC)
        {
            if (MAC == 364)
            {
                
            }
            List<List<int>> finalRoutes = new List<List<int>>();
            List<double> Ps = new List<double>();
            List<int> r0 = new List<int>();
            double p0 = 1;
            finalRoutes.Add(r0);
            Ps.Add(p0);
            for (int i = 0; i < sections.Count;i++)
            {
                if (sections[i].paths.Count == 1)
                {
                    foreach (List<int> r in finalRoutes)
                    {
                        r.AddRange(sections[i].paths[sections[i].paths.Count - 1].nodesIDs);
                        if (i != sections.Count-1)
                            r.RemoveAt(r.Count - 1);
                    }
                }
                else if (sections[i].paths.Count > 1)
                {
                    List<List<int>> AUX = new List<List<int>>();
                    List<double> pAUX = new List<double>();
                    int n = sections[i].paths.Count;
                    for (int j = 0; j < n;j++)
                    {
                        List<List<int>> aux = new List<List<int>>();
                        foreach (List<int> l in finalRoutes)
                        {
                            List<int> auxiliar = new List<int>(l);
                            aux.Add(auxiliar);
                        }
                        List<double> paux = new List<double>(Ps);
                        int k = 0;
                        foreach(List<int> r in aux)
                        {
                            r.AddRange(sections[i].paths[j].nodesIDs);
                            paux[k] = paux[k] * sections[i].paths[j].finalProb;
							if (i != sections.Count - 1)
								r.RemoveAt(r.Count - 1);
                            k++;
						}
                        AUX.AddRange(aux);
                        pAUX.AddRange(paux);
                    }
                    finalRoutes = new List<List<int>>();
					foreach (List<int> l in AUX)
					{
						List<int> auxiliar = new List<int>(l);
                        finalRoutes.Add(auxiliar);
					}
                    Ps = new List<double>(pAUX);
                }
            }

            for (int i = 0; i < finalRoutes.Count;i++)
            {
                Route aux = new Route(MAC, finalRoutes[i], Ps[i]);
                routes.Add(aux);
            }

        }
    }
}
