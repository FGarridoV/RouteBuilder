using System;
using System.IO;
using System.Collections.Generic;

namespace RouteBuilder
{
    public class NetworkReader
    {
        //Class elements
        public int nNodes; 
        public int nLinks;
        public List<double[]> nodesInfo;
        public List<double[]> linksInfo;

        //Constructor
        public NetworkReader(string nodeFileName, string linkFileName)
        {
            try
            {
                StreamReader sr1 = new StreamReader(nodeFileName);
                StreamReader sr2 = new StreamReader(linkFileName);
                this.nNodes = int.Parse(sr1.ReadLine());
                this.nLinks = int.Parse(sr2.ReadLine());
                this.nodesInfo = new List<double[]>(nodeStr_to_list(sr1.ReadToEnd()));
                this.linksInfo = new List<double[]>(LinkStr_to_list(sr2.ReadToEnd()));
                sr1.Close();
                sr2.Close();
            }
            catch
            {
                Console.WriteLine("Error 0001: reading network file ... " + System.DateTime.Now.ToString());
                System.Environment.Exit(0);
            }
            
        }

        //Method 1: Read the nodes caracteristics
        public List<double[]> nodeStr_to_list(string str)
        {
            string[] nodesStr = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<double[]> nodesData = new List<double[]>();

			foreach (string node in nodesStr)
			{
				string[] auxStr = node.Split(',');
                double[] aux = new double[] { int.Parse(auxStr[0]), double.Parse(auxStr[1],System.Globalization.CultureInfo.InvariantCulture), double.Parse(auxStr[2],System.Globalization.CultureInfo.InvariantCulture), int.Parse(auxStr[3])};
				nodesData.Add(aux);
			}

            return nodesData;
        }

        //Method 2: Read the link caracteristics
		public List<double[]> LinkStr_to_list(string str)
		{
            string[] linksStr = str.Split(new string[] { "\r\n", "\n" },StringSplitOptions.RemoveEmptyEntries);
			List<double[]> linksData = new List<double[]>();

			foreach (string link in linksStr)
			{
                string[] auxStr = link.Split(',');
                double[] aux = new double[] {int.Parse(auxStr[0]),int.Parse(auxStr[1]),int.Parse(auxStr[2]) };
                linksData.Add(aux);
			}
			return linksData;
		}
    }

}
