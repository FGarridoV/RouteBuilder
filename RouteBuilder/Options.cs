using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Options
    {
        List<Path> paths;

        public Options(Network net,RealNetwork mn, int nodoSource, int nodoSink, int k)
        {
            List<Path> p1 = new List<Path>(net.YenKsP(nodoSource,nodoSink,k,0,mn));
            List<Path> p2 = new List<Path>(net.YenKsP(nodoSource, nodoSink, k, 1,mn));
            List<Path> p3 = new List<Path>(net.YenKsP(nodoSource, nodoSink, k, 2,mn));
        }
    }
}
