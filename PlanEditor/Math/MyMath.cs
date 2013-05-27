using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.MyMath
{
    public class Helper
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

        public static bool IsHorizontal(double x1, double y1, double x2, double y2)
        {
            double x = Helper.Tan(x1, x2);
            double y = Helper.Tan(y1, y2);

            return (x > y);
        }

        public static double[] GetMinMax(double p1, double p2, double p3, double p4)
        {
            double[] mas = { p1, p2, p3, p4 };
            double max = double.MinValue;
            double min = double.MaxValue;

            for (int i = 0; i < mas.Length; ++i)
            {
                if (mas[i] > max)
                    max = mas[i];
                else if (mas[i] < min)
                    min = mas[i];
            }

            double a = -1;
            double b = -1;
            for (int i = 0; i < mas.Length; ++i)
            {
                if (mas[i] == max || mas[i] == min) continue;

                if (a == -1)
                    a = mas[i];
                else if (b == -1)
                    b = mas[i];
            }
            
            if (a < b)
                return new double[] { a, b };
            else
                return new double[] { b, a };
        }
    }
}


