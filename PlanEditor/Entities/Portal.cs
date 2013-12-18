using PlanEditor.Helpers;
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

        public Place RoomA { get; set; }
        public Place RoomB { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }        
        public bool IsBlocked { get; set; }

        [NonSerialized]
        public List<Cell> Cells = new List<Cell>();

        public void CreateUI(double v) // Функция вызывается, перед тем, как поместить в канвас, для отображения
        {
            var wide = Width / Constants.Sigma;

            var shiftX = Constants.GridStep;
            var shiftY = Constants.GridStep;
            
            if (Orientation == PortalOrient.Horizontal)
                shiftX = wide;
            else
                shiftY = wide;

            var pg = new PathGeometry {FillRule = FillRule.Nonzero};

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = Orientation == PortalOrient.Horizontal ? new Point(Min, v - Constants.GridStep/2) : new Point(v - Constants.GridStep / 2, Min);

            var startPoint = pf.StartPoint;

            for (int i = 0; i < 4; ++i)
            {
                var ls = new LineSegment();

                switch (i)
                {
                    case 0:
                        ls.Point = new Point(startPoint.X + shiftX, startPoint.Y);
                        break;
                    case 1:
                        ls.Point = new Point(startPoint.X + shiftX, startPoint.Y + shiftY);
                        break;
                    case 2:
                        ls.Point = new Point(startPoint.X, startPoint.Y + shiftY);
                        break;
                    case 3:
                        ls.Point = new Point(startPoint.X, startPoint.Y);
                        break;
                }
                pf.Segments.Add(ls);
            }

            var p = new Path {Fill = Colours.LightGray, StrokeThickness = 2, Stroke = Colours.Black, Data = pg};
            UI = p;
        }
    }
}
