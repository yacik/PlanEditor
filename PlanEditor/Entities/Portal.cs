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

        public double Min()
        {
            if (Orientation == PortalOrient.Horizontal)
            {
                return Math.Min(ParentWall.X1, ParentWall.X2);
            }
            else 
            {
                return Math.Min(ParentWall.Y1, ParentWall.Y2);
            }
        }
        public double Max()
        {
            if (Orientation == PortalOrient.Horizontal)
            {
                return Math.Max(ParentWall.X1, ParentWall.X2);
            }
            else
            {
                return Math.Max(ParentWall.Y1, ParentWall.Y2);
            }
        }
        public double Wide
        {
            get 
            {
                return 1 / Data.Sigma;
            }
        }

        public Line ParentWall { get; set; }    

        [NonSerialized]
        public List<Cell> Cells = new List<Cell>();

        public void LoadUI()
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
        
        public void SetUI()
        {
            double shiftX = Data.GridStep;
            double shiftY = Data.GridStep;
            double v = 0.00;
            
            if (Orientation == PortalOrient.Horizontal)
            {
                shiftX = Wide;
                v = ParentWall.Y1;
            }
            else
            {
                shiftY = Wide;
                v = ParentWall.X1;
            }

            var pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            if (Orientation == PortalOrient.Horizontal)
                pf.StartPoint = new Point(Min(), v - Data.GridStep/2);
            else
                pf.StartPoint = new Point(v - Data.GridStep / 2, Min());

            Point startPoint = pf.StartPoint;

            for (int i = 0; i < 4; ++i)
            {
                LineSegment ls = new LineSegment();

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

            Path p = new Path();
            p.Fill = Colours.Red;
            p.StrokeThickness = 2;
            p.Stroke = Colours.Yellow;
            p.Data = pg;
            UI = p;
        }
    }
}
