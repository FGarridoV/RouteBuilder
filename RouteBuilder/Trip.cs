using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Trip
    {
        //Class elements
        public List<Detection> detections;
        public List<int> passingNodes;
        public List<Section> sections;

        //Constructor
        public Trip()
        {
            detections = new List<Detection>();
            passingNodes = new List<int>();
            sections = new List<Section>();
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

            for (int i = 1; i < detections.Count;i++)
            {
                if(passingNodes[passingNodes.Count-1]==detections[i].BSID)
                {
                    continue;
                }
                else
                {
                    passingNodes.Add(detections[i].BSID);
                }
            }
        }

        //Method 4: Add de sections of paths 
        public void add_sections(Network net, int k)
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
                        Section opt = new Section(net, detections[i].BSID, detections[i + 1].BSID, detections[i].time, detections[i + 1].time);
                        sections.Add(opt);
					}

					else
					{
						Section opt = new Section(net, detections[i].BSID, detections[i + 1].BSID,k,detections[i].time, detections[i + 1].time);
						sections.Add(opt);
					}
				}
			}
        }
    }
}
