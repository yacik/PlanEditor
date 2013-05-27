using System.Collections.Generic;
using PlanEditor.Entities;

namespace PlanEditor.RegGrid
{
    public class Cell
    {
        public Cell(double x, double y, int m, int n, int k)
        {
            PosX = x;
            PosY = y;

            M = m;
            N = n;
            K = k;

            Owner = null;
        }

        public int M  { get; private set; }
        public int N { get; private set; }
        public int K { get; private set; }
        public double PosX { get; set; }
        public double PosY { get; set; }

        public List<Cell> Neigh = new List<Cell>();
        public Entity Owner;

        public override string ToString()
        {
            return "M: " + M + " N: " + N + " K: " + K + " owner: " + Owner.ID;
        }
    }
}
