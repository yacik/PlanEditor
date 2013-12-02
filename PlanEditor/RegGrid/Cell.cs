using System.Collections.Generic;
using PlanEditor.Entities;

namespace PlanEditor.RegGrid
{
    public class Cell
    {
        public Cell(double x, double y, int m, int n, int k)
        {
            CenterX = x;
            CenterY = y;

            M = m;
            N = n;
            K = k;

            Owner = null;
        }

        public int M  { get; private set; }
        public int N { get; private set; }
        public int K { get; private set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }

        public List<Cell> Neigh = new List<Cell>();
        public Entity Owner { get; set; }

        public override string ToString()
        {
            return "M: " + M + " N: " + N + " K: " + K + " owner: ";
        }
    }
}
