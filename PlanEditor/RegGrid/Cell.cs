using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlanEditor.RegGrid
{
    public class Cell
    {
        public Cell(double x, double y, int m, int n)
        {
            PosX = x;
            PosY = y;

            M = m;
            N = n;

            Owner = -1;
        }

        public int M  { get; private set; }
        public int N { get; private set; }
        public double PosX { get; private set; }
        public double PosY { get; private set; }
        public int ID { get; set; }       
        public List<Cell> Neigh = new List<Cell>();
        public int Owner;
    }
}
