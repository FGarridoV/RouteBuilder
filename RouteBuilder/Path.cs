using System;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class Path : IComparable<Path>
    {
		//Class elements
        public List<int> nodesIDs;
		public List<Node> nodes;
		public List<Link> links;
        public double totalCost;

        public double P_totalTravelTime;
        public Histogram TTT;
        public int startNodeCounts;
        public int endNodeCounts;

        public double P_missedDetections;
        public double distance;

        public double P_routeUses;
        public double meanF;

        public double finalProb;


        //Constructor
        public Path(List<int> nodesIDs, Network net)
        {
            this.nodesIDs = new List<int>(nodesIDs);
            nodes = new List<Node>();
            links = new List<Link>();
            this.totalCost = 0;
            this.distance = 0;

            for (int i = 0; i < nodesIDs.Count; i++)
            {
                nodes.Add(net.nodeByID(nodesIDs[i]));

                if (i < nodesIDs.Count - 1)
                    links.Add(net.LinkByNodesID(nodesIDs[i], nodesIDs[i + 1]));
            }
            add_totalCost();
        }

        //Method 1: Compare 2 paths by nodes ID
        public bool Equals(Path p)
        {
            if(this.nodesIDs.Count == p.nodesIDs.Count)
            {
                for (int i = 0; i < this.nodesIDs.Count;i++)
                {
                    if (this.nodesIDs[i] == p.nodesIDs[i])
                        continue;
                    else
                        return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        //Method 2: Compare 2 list of ints
        public static bool areEquals(List<int> nodes1,List<int> nodes2) 
        {
			if (nodes1.Count == nodes2.Count)
			{
				for (int i = 0; i < nodes1.Count; i++)
				{
					if (nodes1[i] == nodes2[i])
						continue;
					else
						return false;
				}
			}

			else
			{
				return false;
			}
			return true;
        }

        //Method 3: Return a subgroup of nodes ints id
        public List<int> some_nodes(int start_index, int last_index)
        {
            List<int> someNodes = new List<int>();
            for (int i = start_index; i <= last_index;i++)
            {
                someNodes.Add(nodesIDs[i]);
            }
            return someNodes;
        }

        //Method 4: Add the total cost to the Path
        public void add_totalCost()
        {
            foreach(Link l in links)
            {
                this.totalCost += l.mainCost;
                this.distance += l.distanceCost;
            }
        }

        //Method 5: Compare to path with the cost
        public int CompareTo(Path other)
        {
            if (this.totalCost > other.totalCost)
				return 1;

            else if (this.totalCost < other.totalCost)
				return -1;

			else
				return 0;
        }

        //Method 6: Assing the Convolved Total travel time
        private void set_TTT(int period)
        {
            if (nodesIDs.Count > 1)
            {
                List<double> BinRanges = new List<double>();
                List<DwellTimes> dts = new List<DwellTimes>();
                List<TravelTimes> tts = new List<TravelTimes>();
                for(int i = 1; i<nodes.Count-1;i++)
                {
                    dts.Add(nodes[i].dt_at_specific_period(period));
                    BinRanges.Add(dts[dts.Count-1].optimalA);
                }

                foreach (Link l in links)
				{
					tts.Add(l.tt_at_specific_period(period));
					BinRanges.Add(tts[tts.Count - 1].optimalA);
				}
                double optimalRangeBin = optimal_a_in_convolution(BinRanges);

                List<Histogram> toConv = new List<Histogram>();

                foreach (DwellTimes dt in dts)
				{
                    Histogram h = new Histogram(dt, optimalRangeBin).gen_distribution();
                    toConv.Add(h);
				}

                foreach(TravelTimes tt in tts)
                {
                    Histogram h = new Histogram(tt, optimalRangeBin).gen_distribution();
                    toConv.Add(h);
                }

                this.TTT = Histogram.multi_convolution(toConv);
            }

        }

        //Method 7: Return the optimal binrange in a set of ranges
        public double optimal_a_in_convolution(List<double> aes)
        {
            double sum = 0;
            foreach(double a in aes)
            {
                sum = sum + a;
            }
            return sum / aes.Count;
        }

        //Method 8: return the error in tobs
        public double measure_error(int period)
        {
            Histogram A = new Histogram(nodes[0].dt_at_specific_period(period),nodes[0].dt_at_specific_period(period).optimalA).gen_distribution();
            Histogram B = new Histogram(nodes[nodes.Count-1].dt_at_specific_period(period), nodes[nodes.Count-1].dt_at_specific_period(period).optimalA).gen_distribution();
            return A.get_mean() / (startNodeCounts + 1) + B.get_mean() / (endNodeCounts + 1);
        }

        public void set_P_ttt(double T_obs, int period)
        {
            this.set_TTT(period);
            this.P_totalTravelTime = this.TTT.integral(T_obs - measure_error(period), T_obs);
        }

        public void set_P_md(double T_obs)
        {
            double speed = distance / T_obs;
            double p_v = p_from_v(speed);
            double n = nodesIDs.Count - 2;
            this.P_missedDetections = Math.Pow((1 - p_v),n);
        }

        public void set_fs(Scenario sc, int period)
        {
            int n = nodesIDs.Count - 2;
            double fs = 0;
            for (int k = 1; k <= n; k++)
            {
                double[] DBkv = sc.get_DB_k_and_v(this, k, sc.T, period);
                double combinatory = comb(n, k);
                double pvk = Math.Pow(p_from_v(DBkv[1]), k);
                double npvk = Math.Pow(1-p_from_v(DBkv[1]), n-k);
                double f = DBkv[0] / (combinatory * pvk * npvk);
                fs += f;
            }
            meanF = fs / n;
        }

        public static int factorial(int n)
        {
			int fact = 1;
			for (int i = n; i > 0; i--)
			{
				fact = fact * i;
			}
            return fact;
        }

        public static int comb(int n, int k)
        {
            int num = factorial(n);
            int den = factorial(k) * factorial(n - k);
            return num / den;
        }

        public void set_P_ru(double val)
        {
            this.P_routeUses = val;
        }

        public double p_from_v(double speed)
        {
            speed = speed * 3.6;
            if (speed >= 0 && speed <= 80)
                return -0.01 * speed + 0.8;
            
            else
                return 0;
        }

        public void set_startNodeCount(int counts)
        {
            startNodeCounts = counts;
        }

		public void set_endNodeCount(int counts)
		{
			endNodeCounts = counts;
		}

        public void set_final_prob(double val)
        {
            this.finalProb = val;
        }
    }
}
