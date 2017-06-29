using System;
using System.IO;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class DataBaseReader
    {
        //Class elements
        public List<double[]> BTData;

        //Constructor
        public DataBaseReader(string fileName)
        {
            try
            {
                StreamReader sr = new StreamReader(fileName);
                this.BTData = new List<double[]>(dataStr_to_list(sr.ReadToEnd()));
                sr.Close();
            }

            catch
            {
				Console.WriteLine("Error 0002: reading database file");
				System.Environment.Exit(0);
            }
        }

        //Method 1: Returns a double list of BT data from string 
        public List<double []> dataStr_to_list(string str)
        {
			string[] dataStr = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			List<double[]> data = new List<double[]>();

            foreach (string info in dataStr)
			{
				string[] auxData = info.Split(',');
				double[] aux = new double[] { double.Parse(auxData[0]), double.Parse(auxData[1]), timeStr_to_double(auxData[2])};
				data.Add(aux);
			}
			return data;
        }

        //Method 2: Returns a double of seconds of a time in str in respect of 00:00:00  
        public double timeStr_to_double(string str)
        {
            string[] times = str.Split(':');
            double time = double.Parse(times[0]) * 3600 + double.Parse(times[1]) * 60 + double.Parse(times[2]);
            return time;
        }
    }
}
