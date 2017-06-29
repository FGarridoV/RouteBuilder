﻿using System;
using System.Collections.Generic;
namespace RouteBuilder
{
    public class Link
    {
		//Class elements
        public int ID;
		public Node tailNode;
		public Node headNode;
		public double distanceCost;
		public double edgesCost;
        public double angularCost;
        public double mainCost;
        public List<TravelTimes> tTimes;

        //Constructor from a RealLink
        public Link(RealLink l)
        {
            this.ID = l.ID;
            this.distanceCost = l.dijkstraDistance;
            this.edgesCost = l.dijkstraNodes;
            this.tTimes = new List<TravelTimes>();
        }

        //Constructor to make copies
        public Link(Link l)
        {
			this.ID = l.ID;
            this.distanceCost = l.distanceCost;
            this.edgesCost = l.edgesCost;
            this.angularCost = l.angularCost;
            this.mainCost = l.mainCost;
			this.tTimes = new List<TravelTimes>();
        }

        //Method 1: Add the tail node element
        public void add_tailNode(Node n)
        {
            this.tailNode = n;
        }

        //Method 2: Add the head node element
		public void add_headNode(Node n)
		{
            this.headNode = n;
		}

        //Method 3: Set a travel time at period specified  
		public void set_travel_time_at_period(int period, double time)
		{
			if (exist_period(period))
			{
				foreach (TravelTimes t in tTimes)
				{
					if (t.ID_period == period)
						t.add_tTime(time);
				}
			}
			else
			{
                TravelTimes tts = new TravelTimes(period);
				tts.add_tTime(time);
                tTimes.Add(tts);
			}

		}

        //Method 4: Determine if the period exist in the times
		public bool exist_period(int period)
		{
			foreach (TravelTimes t in tTimes)
			{
				if (t.ID_period == period)
					return true;
			}
			return false;
		}


    }
}
