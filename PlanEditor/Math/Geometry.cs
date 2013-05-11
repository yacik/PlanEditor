using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.MyMath
{
    public class Geometry
    {
        public static bool IsCollide(double x, double y, List<double> pointsX, List<double> pointsY)
        {
            bool c = false;

            int size = pointsX.Count;
            for (int i = 0, j = (size - 1); i < size; j = i++)
            {
                if (((pointsY[i] > y) != (pointsY[j] > y)) && (x < (pointsX[j] - pointsX[i]) * (y - pointsY[i]) / (pointsY[j] - pointsY[i]) + pointsX[i]))
                {
                    c = !c;
                }
            }

            return c;
        }

        public static double Tan(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }

        public static double Tan(double x1, double x2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2));
        }
    }
}


