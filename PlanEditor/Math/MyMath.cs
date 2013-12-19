using System;
using System.Collections.Generic;
using PlanEditor.Helpers;

namespace PlanEditor.MyMath
{
    public class Helper
    {
        //Что с чем сталкивается?
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

        // Перевод метров в пиксели
        public static double MetersToPxl(double meters)
        {
            return meters/Constants.Sigma;
        }

        // Что здесь считается. Тангенс? для чего это значение?
        public static double Tan(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }
        // Переопределенный метод Tan, для двух чисел.
        // Почему только для иксов?
        public static double Tan(double x1, double x2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2));
        }
        // Определение горизонтальности. Горизонтальности чего?
        public static bool IsHorizontal(double x1, double y1, double x2, double y2)
        {
            double x = Helper.Tan(x1, x2);
            double y = Helper.Tan(y1, y2);

            return (x > y);
        }
        // Формирование массива из входных данных по убыванию
        // Где используется? С какой целью?
        public static double[] GetMinMax(double x1, double x2)
        {
            return x1 < x2 ? new[] { x1, x2 } : new[] { x2, x1 };
        }
        // Формирование массива из входных данных по убыванию из четырех точек
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


            return (a < b) ? new[] { a, b } : new[] { b, a };
        }
    }
}


