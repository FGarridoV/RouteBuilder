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
                double[] values = new double[] {d[0],d[1],d[2]/(s*a)};
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

        public double getVal(double val)
        {
            if (val <= 0)
                return 0;
            else if (val > this.data[data.Count-1][1])
                return 0;
            else
                return this.data[bin_pos(val)][2];
            
        }

        public static Histogram convolution(Histogram h1, Histogram h2)
        {
            Histogram convolved = new Histogram(h1, h2, h1.a);
            List<double> boundValues = new List<double>();
            boundValues.Add(0);
            double n = 1;
            foreach(double[] boundVal in convolved.data)
            {
                double evalf = 0;
                double k = 0;
                foreach(double[] bounds in convolved.data)
                {
                    double nF = (k * convolved.a + (k + 1) * convolved.a)/2;
                    double nG = ((n - k - 1) * convolved.a + (n - k) * convolved.a) / 2;
                    evalf += h1.getVal(nF) * h2.getVal(nG) * convolved.a;
                    k++;
                }
                boundValues.Add(evalf);
                n++;
            }

            for (int i = 0; i < convolved.data.Count;i++)
            {
                convolved.data[i][2]=(boundValues[i]+boundValues[i+1])/2;
            }

            return convolved;
        }

        public static Histogram multi_convolution(List <Histogram> Hs)
        {
            Histogram ConvAux = convolution(Hs[0], Hs[1]);

            for (int i = 2; i < Hs.Count-1;i++)
            {
                ConvAux = convolution(ConvAux, Hs[i]);
            }

            return ConvAux;
        }

        public double get_mean()
        {
            double sum = 0;
            for (int i = 0; i < this.data.Count;i++)
            {
                sum += (Math.Pow(this.a,2)/2)*this.data[i][2] * (2 * i + 1);
            }
            return sum;
        }

        public double integral(double fromVal, double toValue)
        {
            if (fromVal > this.data[this.data.Count - 1][1])
                return 0;
            if (toValue > this.data[this.data.Count - 1][1])
                toValue = this.data[this.data.Count - 1][1];
            int startBin = bin_pos(fromVal);
            int endBin = bin_pos(toValue);
            double area = 0;
            if (startBin == endBin)
            {
                return this.data[startBin][2] * (toValue - fromVal);
            }
            for (int i = startBin; i <= endBin; i++)
            {
                if(i==startBin)
                {
                    area += (this.data[i][1] - fromVal) * this.data[i][2];
                }
                else if(i==endBin)
                {
                    area += (toValue - this.data[i][0]) * this.data[i][2];
                }
                else
                {
                    area += this.a * this.data[i][2];
                }
            }
            return area;
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
