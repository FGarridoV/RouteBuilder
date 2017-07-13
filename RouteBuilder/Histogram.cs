using System;
using System.Collections.Generic;
using System.IO;

namespace RouteBuilder
{
    public class Histogram
    {
        int nBins;
        double a;
        double min;
        double max;
        List<double[]> data;

        public Histogram(DwellTimes dts, double a)
        {
            this.min = 0;
            this.max = dts.max_value();
            dts.times.Sort();
            data = new List<double[]>();
            int i = 0;
            while (min + a * i < max)
            {
                double[] aux = new double[] { min + a * i, min + a * (i + 1), 0 };
                data.Add(aux);
                i++;
            }
            this.a = a;
            this.nBins = data.Count;

            foreach(double t in dts.times)
            {
                data[bin_pos(t)][2]++;
            }
        }

		public Histogram(TravelTimes tts, double a)
		{
			this.min = 0;
			this.max = tts.max_value();
			tts.times.Sort();
			data = new List<double[]>();
			int i = 0;
			while (min + a * i < max)
			{
				double[] aux = new double[] { min + a * i, min + a * (i + 1), 0 };
				data.Add(aux);
				i++;
			}
			this.a = a;
			this.nBins = data.Count;

			foreach (double t in tts.times)
			{
				data[bin_pos(t)][2]++;
			}
		}

        private Histogram()
        {
            data = new List<double[]>();
        }

		public int bin_pos(double t)
		{
            int pos = (int)Math.Ceiling(((t - min) / a)-1);
			return pos;
		}

        private Histogram(Histogram h1, Histogram h2, double a)
        {
			this.min = 0;
            this.max = h1.data[h1.data.Count - 1][1] + h2.data[h2.data.Count - 1][1];
			data = new List<double[]>();
			int i = 0;
			while (min + a * i < max)
			{
				double[] aux = new double[] { min + a * i, min + a * (i + 1), 0 };
				data.Add(aux);
				i++;
			}
			this.a = a;
			this.nBins = data.Count;
        }

        public Histogram gen_distribution()
        {
            Histogram h = new Histogram();
            h.nBins = this.nBins;
            h.min = this.min;
            h.max = this.max;
            h.a = this.a;
            int s = this.total_data();
            foreach(double[] d in this.data)
            {
                double[] values = new double[] {d[0],d[1],d[2]/s};
                h.data.Add(values);
            }
            return h;
        }

        public int total_data()
        {
            double sum = 0;
            foreach(double[] d in data)
            {
                sum = sum + d[2];
            }
            return (int)sum;
        }

        public double getVal(double t)
        {
            int interval = bin_pos(t);

            if (interval < 0)
                return 0;

            else if (interval > nBins)
                return 0;

            else
                return data[interval][2];
        }

        public static Histogram convolution(Histogram h1, Histogram h2)
        {
            Histogram convolved = new Histogram(h1, h2, h1.a);

            for (int i = 0; i < convolved.data.Count; i++)
			{
                double val = 0;
                for (int m = 0; m < convolved.data.Count;m++)
                {
                    val = val + h1.getVal(m) * h2.getVal(i - m);
                }
                convolved.data[i][2] = val;
			}
            return convolved;
        }

        public void export(string name)
        {
            StreamWriter sw = new StreamWriter(name);
            foreach(double[] dat in data)
            {
                sw.WriteLine(dat[2]);
            }
            sw.Close();
        }

    }
}
