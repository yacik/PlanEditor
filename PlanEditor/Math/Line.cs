using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.MyMath
{
    public class Line
    {
        public Line(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = Y2;
        }

        public override string ToString()
        {            
            return "[" + X1 + ", " + Y1 + "; " + X2 + ", " + Y2 + "]";
        }

        public double X1 { get; private set; }
        public double Y1 { get; private set; }
        public double X2 { get; private set; }
        public double Y2 { get; private set; }
    }
}
