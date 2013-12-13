/*
 * Класс с разными вспомогательными функциями
 */

using System;
using System.Linq;
using PlanEditor.Entities;
using PlanEditor.MyMath;

namespace PlanEditor.Helpers
{
    public class StuffHelper
    {
        // Определеяет ближаюшую точку
        public static bool IsNearest(double a1, double a2, double b1, double b2)
        {
            var v1 = Helper.Tan(a1, b1);
            var v2 = Helper.Tan(a1, b2);
            var v3 = Helper.Tan(a2, b1);
            var v4 = Helper.Tan(a2, b2);

            var mas = new[] { v1, v2, v3, v4 };
            var min = mas.Concat(new[] { double.MaxValue }).Min();

            return min < Constants.GridStep;
        }

        // Определяет соседние комнаты
        public static bool DefineNeighRooms(Place place1, Place place2)
        {
            var mas1 = new[] { double.MaxValue, double.MinValue, double.MaxValue, double.MinValue };
            var mas2 = new[] { double.MaxValue, double.MinValue, double.MaxValue, double.MinValue };

            foreach (var line in place1.Lines)
            {
                var x1 = Math.Min(line.X1, line.X2);
                var x2 = Math.Max(line.X1, line.X2);
                var y1 = Math.Min(line.Y1, line.Y2);
                var y2 = Math.Max(line.Y1, line.Y2);

                if (mas1[0] > x1) mas1[0] = x1;
                if (mas1[1] < x2) mas1[1] = x2;
                if (mas1[2] > y1) mas1[2] = y1;
                if (mas1[3] < y2) mas1[3] = y2;
            }

            foreach (var line in place2.Lines)
            {
                var x1 = Math.Min(line.X1, line.X2);
                var x2 = Math.Max(line.X1, line.X2);
                var y1 = Math.Min(line.Y1, line.Y2);
                var y2 = Math.Max(line.Y1, line.Y2);

                if (mas2[0] > x1) mas2[0] = x1;
                if (mas2[1] < x2) mas2[1] = x2;
                if (mas2[2] > y1) mas2[2] = y1;
                if (mas2[3] < y2) mas2[3] = y2;
            }

            var ay = mas1[3] - mas2[2];
            var by = mas2[2] - mas1[3];

            var tmpY = Math.Min(ay, by);
            var y = Math.Sqrt(tmpY * tmpY);

            var ax = mas1[1] - mas2[0];
            var bx = mas2[0] - mas1[1];

            var tmpX = Math.Min(ax, bx);
            var x = Math.Sqrt(tmpX * tmpX);

            return x > y;
        }
    }
}
