using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

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

        public void set_P_ru_to_paths_full(Scenario sc)
        {
            double totalFlow = 0;
            foreach (Path p in paths)
			{
                p.set_fs_full(sc,period);
                totalFlow += p.meanF;
			}

			foreach (Path p in paths)
			{
                p.set_P_ru(p.meanF/totalFlow);
			}
        }

		public void set_P_ru_to_paths_equi(Scenario sc)
		{
            foreach (Path p in paths)
            {
                p.set_P_ru(1.0/paths.Count);
            }
		}

		public void set_P_ru_to_paths_complete(Scenario sc)
		{
			
			foreach (Path p in paths)
			{
                p.set_fs_if_unique(sc, period);
			}
            set_flows_by_system(sc,period);

            double totalFlow = 0;
			foreach (Path p in paths)
			{
				totalFlow += p.meanF;
			}

			foreach (Path p in paths)
			{
				p.set_P_ru(p.meanF / totalFlow);
			}

		}
		public void set_P_ru_to_paths_chung2014(Scenario sc)
		{

			List<Path> options = new List<Path>();
			foreach (Path p in paths)
			{
				p.set_P_ru(0);
				options.Add(p);
			}
			options.Sort((x, y) => x.distance.CompareTo(y.distance));
			options[0].set_P_ru(1);
		}

		public void set_P_ru_to_paths_chung2017(Scenario sc)
		{
            
            List<Path> options = new List<Path>();
			foreach (Path p in paths)
			{
                p.set_P_ru(0);
                options.Add(p);
			}
            options.Sort((x, y) => x.distance.CompareTo(y.distance));
            options[0].set_P_ru(1);
		}

        public void set_flows_by_system(Scenario sc, int period)
        {
            List<int> nodesEval = new List<int>(nodes_not_uniques());
            if (nodesEval.Count > 0)
            {
                List<double[]> equations = new List<double[]>();
                List<double> ress = new List<double>();

                foreach (int n in nodesEval)
                {
                    double[] DBv = sc.get_DB_node_and_v(paths[0], n, period);
                    double Pv = Path.p_from_v(DBv[1]);
                    List<int> path_pos = new List<int>(is_member_of(n));
                    int[] nX = number_of_missing_flows(path_pos);
                    if (nX[0] == 1)
                    {
                        double sumatory = 0;
                        foreach (int i in path_pos)
                        {
                            if (paths[path_pos[i]].meanF >= 0)
                            {
                                sumatory += paths[path_pos[i]].meanF * Pv * Math.Pow(1 - Pv, paths[path_pos[i]].nodesIDs.Count - 3);
                            }
                        }
                        double f = (DBv[0] - sumatory) / (Pv * Math.Pow(1 - Pv, paths[nX[1]].nodesIDs.Count - 3));
                        paths[nX[1]].set_meanF(f);
                    }

                    else if (nX[0] > 1)
                    {
                        double[] eq = new double[paths.Count];
                        for (int i = 0; i < path_pos.Count;i++)
                        {
                            if(paths[path_pos[i]].meanF < 0)
                            {
                                eq[path_pos[i]] = Pv * Math.Pow(1-Pv,paths[path_pos[i]].nodesIDs.Count-3);
                            }
                            else
                            {
                                eq[path_pos[i]] = 0;
                            }
                        }
						double res = DBv[0];
                        for (int i = 0; i < path_pos.Count; i++)
                        {
							if (paths[path_pos[i]].meanF >= 0)
							{
                                res-=paths[path_pos[i]].meanF*Pv * Math.Pow(1 - Pv, paths[path_pos[i]].nodesIDs.Count - 3);
							}
                        }
                        equations.Add(eq);
                        ress.Add(res);
                    }
                }

                while(number_Xs(equations)>equations.Count)
                {
                    int rank = 1;
					int nPath = f_times_repeat(equations, rank)[0];
					double[] DBv = sc.get_DB_Full_and_v(paths[nPath], period);
                    rank++;
                    while(Math.Abs(DBv[0]) < 0.000001 && rank<=number_Xs(equations) && f_times_repeat(equations, rank)[1]>0)
                    {
                        nPath = f_times_repeat(equations, rank)[0];
                        DBv = sc.get_DB_Full_and_v(paths[nPath], period);
                        rank++;
                    }
                    paths[nPath].set_fs_full(sc, period);

                    List<int> nPathsToProve = new List<int>(update_system_equations(equations,ress,nPath));

                    while(nPathsToProve.Count>0)
                    {
                        List<int> aux = new List<int>(update_system_equations(equations, ress, nPathsToProve[0]));
                        nPathsToProve.RemoveAt(0);
                        nPathsToProve.AddRange(aux);
                    }
                }
                double[] ressA = ress.ToArray();
                Matrix<double> A = CreateMatrix.DenseOfRowArrays<double>(equations);
                Matrix<double> B = CreateMatrix.DenseOfColumnArrays<double>(ressA);
                Matrix<double> X = A.Inverse() * B;

                for (int i = 0; i < paths.Count;i++)
                {
                    if(paths[i].meanF<0)
                    {
                        paths[i].set_meanF(X[i, 0]);
                    }
                }

            }




        }

        public List<int> update_system_equations(List<double[]> equations, List<double> ress, int nPath)
        {
            List<int> nPaths_flow_assigned = new List<int>();
            for (int i = 0; i < equations.Count; i++)
			{
				if (equations[i][nPath] > 0)
				{
					double newVal = equations[i][nPath] * paths[nPath].meanF;
					equations[i][nPath] = 0;
					ress[i] -= newVal;
                    if (ress[i] < 0)
                        ress[i] = 0;
				}
			}

			for (int i = 0; i < equations.Count; i++)
			{
				List<int> x_pos = new List<int>(Xs_pos(equations[i]));
				if (x_pos.Count == 1)
				{
					paths[x_pos[0]].set_meanF(ress[i] / equations[i][x_pos[0]]);
					equations.RemoveAt(i);
					ress.RemoveAt(i);
                    nPaths_flow_assigned.Add(x_pos[0]);
				}
			}
            return nPaths_flow_assigned;
        }

        public List<int> Xs_pos(double[] eq)
        {
            List<int> pos = new List<int>();
            for (int i = 0; i < eq.Length;i++)
            {
                if (eq[i] > 0)
                    pos.Add(i);
            }
            return pos;
        }

        public int[] f_times_repeat(List<double[]> matrix, int rank)
        {
            List<int> repeatsColl = new List<int>();
            for (int i = 0; i < matrix[0].Length;i++)
            {
                int repeats = 0;
                for (int j = 0; j < matrix.Count; j++)
                {
                    if (matrix[j][i] > 0)
                    {
                        repeats++;
                    }
                }
                repeatsColl.Add(repeats);
            }
            List<int[]> aux = new List<int[]>();
            for (int i = 0; i < repeatsColl.Count;i++)
            {
                int[] vals = new int[] { repeatsColl[i], i };
                aux.Add(vals);
            }

            aux.Sort((int[] x, int[] y) => x[0].CompareTo(y[0]));
			aux.Reverse();
            return new int[] { aux[rank-1][1], aux[rank-1][0] };
        }

        public int number_Xs(List<double[]> matrix)
        {
            int numb_Xs = 0;
            for (int i = 0; i < matrix[0].Length;i++)
            {
                for (int j = 0; j < matrix.Count;j++)
                {
                    if (matrix[j][i] > 0)
                    {
                        numb_Xs++;
                        break;
                    }
                }
            }
            return numb_Xs;
        }

        public int[] number_of_missing_flows(List<int> path_pos)
        {
            int num = 0;
            int pathX = -1;
            foreach(int i in path_pos)
            {
                if (Math.Abs(paths[i].meanF - -1) < 0.0000001)
                {
                    num++;
                    pathX = i;
                }
            }
            return new int[] {num, pathX};
        }

        public List<int> is_member_of(int node_id)
        {
            List<int> path_pos = new List<int>();
            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = 0; j < paths[i].nodesIDs.Count;j++)
                {
                    if(paths[i].nodesIDs[j]==node_id)
                    {
                        path_pos.Add(i);
                    }
                }
            }
            return path_pos;
        }

        public List<int> nodes_not_uniques()
        {
            List<int> nodesRes = new List<int>(); 
            foreach(Path p in paths)
            {
                for (int i = 0; i < p.nodesIDs.Count;i++)
                {
                    if (p.repeatsIDs[i] > 0 && !nodesRes.Contains(p.nodesIDs[i]))
                        nodesRes.Add(p.nodesIDs[i]);
                }
            }
            return nodesRes;
        }

        public void apply_BayesianInference(Scenario sc, int version)
        {
            set_P_ttt_to_paths();
            set_P_md_to_paths();
            if (version == 1)
                set_P_ru_to_paths_equi(sc);
            else if (version == 2)
                set_P_ru_to_paths_full(sc);
            else if (version == 3)
                set_P_ru_to_paths_complete(sc);
            else if (version == 4)
                set_P_ru_to_paths_chung2014(sc);
            else
                set_P_ru_to_paths_chung2017(sc);

            ////////////
            double ver = 0;
            foreach(Path p in paths)
            {
                ver += p.P_totalTravelTime*p.P_missedDetections*p.P_routeUses;
            }
            if(Math.Abs(ver) < 0.00000001)
            {
                
            }
            /////////// 


            double totalProb = 0; 
            foreach(Path p in paths)
            {
                if (version == 1 || version == 2 || version == 3)
                    totalProb += p.P_routeUses * p.P_totalTravelTime * p.P_missedDetections;
                else
                    totalProb += p.P_routeUses;

            }

            if(Math.Abs(totalProb) < 0.000001)
            {
                
            }
			foreach (Path p in paths)
			{
                if (version == 1 || version == 2 || version == 3)
                    p.set_final_prob(p.P_routeUses * p.P_totalTravelTime * p.P_missedDetections/totalProb);
                else
                    p.set_final_prob(p.P_routeUses/ totalProb);
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
