using PlanEditor.RegGrid;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Portal : Entity
    {
        public Portal()
        {
            Type = EntityType.Portal;
        }

        public enum PortalOrient { Vertical, Horizontal }
        public PortalOrient Orientation { get; set; }

        public Place RoomB { get; set; }
        public Place RoomA { get; set; }

        public double min;
        public double max;

        [NonSerialized]
        public List<Cell> Cells = new List<Cell>();

        public void CreateUI()
        {
            var pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(ExportX[0], ExportY[0]);
            Point startPoint = pf.StartPoint;

            for (int i = 1, j = 1; i < ExportX.Count && j < ExportY.Count; ++i, ++j)
            {
                LineSegment ls = new LineSegment();
                ls.Point = new Point(ExportX[i], ExportY[j]);
                pf.Segments.Add(ls);
            }

            Path p = new Path();
            p.Fill = Colours.LightGray;
            p.StrokeThickness = 2;
            p.Stroke = Colours.Green;
            p.Data = pg;
            UI = p;
        }
    }
}
