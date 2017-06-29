using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Options
    {
        //Class elements
        List<Path> paths;

        //Constructor
        public Options(Network net, int nodoSource, int nodoSink, int k)
        {
            paths = new List<Path>();

            List<Path> p1 = new List<Path>(net.YenKsP(nodoSource, nodoSink, k, 0));
            foreach (Path p in p1)
            {
                Path aux = new Path(p.nodesIDs, net);
                paths.Add(aux);
            }

            List<Path> p2 = new List<Path>(net.YenKsP(nodoSource, nodoSink, k, 1));
            foreach (Path p in p2)
            {
                if(is_new_path(p))
                {
					Path aux = new Path(p.nodesIDs, net);
					paths.Add(aux); 
                }
            }

            List<Path> p3 = new List<Path>(net.YenKsP(nodoSource, nodoSink, k, 2));
            foreach (Path p in p3)
            {
                if (is_new_path(p))
				{
					Path aux = new Path(p.nodesIDs, net);
					paths.Add(aux);
				}
            }
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
