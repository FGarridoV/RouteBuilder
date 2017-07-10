using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Section
    {
        //Class elements
        List<Path> paths;
        double timeStart;
        double timeEnd;

        //Constructor without inference
        public Section(Network net, int nodeSource, int nodeSink, double timeStart, double timeEnd)
        {
            this.paths = new List<Path>();
            List<int> nodesID = new List<int>();
            nodesID.Add(nodeSource);
            nodesID.Add(nodeSink);
            Path p = new Path(nodesID, net);
            paths.Add(p);
            this.timeStart = timeStart;
            this.timeEnd = timeEnd;
        }

        //Constructor for inference
        public Section(Network net, int nodeSource, int nodeSink, int k, double timeStart, double timeEnd)
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
    }
}
