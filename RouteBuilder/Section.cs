using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Section
    {
        //Class elements
        public List<Path> paths;
        public double timeStart;
        public double timeEnd;
        public int period;

        //Constructor without inference
        public Section(Network net, int nodeSource, int nodeSink, double timeStart, double timeEnd, double T, int sCount, int eCount)
        {
            this.paths = new List<Path>();
            List<int> nodesID = new List<int>();
            nodesID.Add(nodeSource);
            nodesID.Add(nodeSink);
            Path p = new Path(nodesID, net);
            paths.Add(p);
            this.timeStart = timeStart;
            this.timeEnd = timeEnd;
            this.period = (int)Math.Ceiling(timeStart / T);

			foreach (Path pa in paths)
			{
				pa.set_startNodeCount(sCount);
				pa.set_endNodeCount(eCount);
			}
        }

        //Constructor for inference
        public Section(Network net, int nodeSource, int nodeSink, int k, double timeStart, double timeEnd, double T, int sCount, int eCount)
        {
            this.paths = new List<Path>();

            List<Path> p1 = new List<Path>(net.YenKsP(nodeSource, nodeSink, k, 0));
            foreach (Path p in p1)
            {
                Path aux = new Path(p.nodesIDs, net);
                paths.Add(aux);
            }

            List<Path> p2 = new List<Path>(net.YenKsP(nodeSource, nodeSink, k, 1));
            foreach (Path p in p2)
            {
                if(is_new_path(p))
                {
					Path aux = new Path(p.nodesIDs, net);
					paths.Add(aux); 
                }
            }

            List<Path> p3 = new List<Path>(net.YenKsP(nodeSource, nodeSink, k, 2));
            foreach (Path p in p3)
            {
                if (is_new_path(p))
				{
					Path aux = new Path(p.nodesIDs, net);
					paths.Add(aux);
				}
            }

			this.timeStart = timeStart;
			this.timeEnd = timeEnd;
            this.period = (int)Math.Ceiling(timeStart / T);

            foreach(Path p in paths)
            {
                p.set_startNodeCount(sCount);
                p.set_endNodeCount(eCount);
            }
            this.set_repeats();
        }

        //Method 1: Return true if the path doesnt exist
        public bool is_new_path(Path p)
        {
            foreach(Path q in paths)
            {
                if(q.Equals(p))
                {
                    return false;
                }
            }
            return true;
        }

        public void set_P_ttt_to_paths()
        {
            foreach(Path p in paths)
            {
                p.set_P_ttt(timeEnd - timeStart, this.period);
            }
        }

        public void set_P_md_to_paths()
        {
			foreach (Path p in paths)
			{
				p.set_P_md(timeEnd - timeStart);
			}
        }

        public void set_P_ru_to_paths_old(Scenario sc)
        {
            double totalFlow = 0;
            foreach (Path p in paths)
			{
                p.set_fs_old(sc,period);
                totalFlow += p.meanF;
			}

			foreach (Path p in paths)
			{
                p.set_P_ru(p.meanF/totalFlow);
			}
        }

        public void apply_BayesianInference_old(Scenario sc)
        {
            set_P_ttt_to_paths();
            set_P_md_to_paths();
            set_P_ru_to_paths_old(sc);

            double totalProb = 0;
            foreach(Path p in paths)
            {
                totalProb += p.P_routeUses * p.P_totalTravelTime * p.P_missedDetections;
            }

			foreach (Path p in paths)
			{
                p.set_final_prob(p.P_routeUses * p.P_totalTravelTime * p.P_missedDetections/totalProb);
			}
        }

        public void apply_ObiouslyInference()
        {
			foreach (Path p in paths)
			{
				p.set_final_prob(1);
			}
        }

        public void set_repeats()
        {
            foreach(Path p in paths)
            {
                for (int i = 0; i < p.nodesIDs.Count;i++)
                {
                    int counts = 0;
                    if(i==0 || i == p.nodesIDs.Count - 1)
                        counts = -1;
                    else
                    {
						foreach (Path pa in paths)
						{
                            if (pa.has_node(p.nodesIDs[i]) == true)
							{
								counts++;
							}
						}
                        counts--;
                    }
                    p.add_count_nodes_id(counts);
                }
            }
        }
    }
}
