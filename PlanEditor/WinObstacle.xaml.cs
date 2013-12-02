using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Entities;
using PlanEditor.Helpers;

namespace PlanEditor
{
    public partial class WinObstacle : Window
    {
        private readonly Place _owner;
        private readonly Obstacle _obstacle;

        private Point _min = new Point(double.MaxValue, double.MaxValue);
        private Point _max = new Point(double.MinValue, double.MinValue);

        public WinObstacle(Place owner)
        {
            InitializeComponent();
            _owner = owner;
            SetMinMax();
            BtnOK.IsEnabled = false;
        }

        public WinObstacle(Obstacle obstacle)
        {
            InitializeComponent();
            _obstacle = obstacle;
            _owner = _obstacle.Owner;
            SetMinMax();
            BtnOK.IsEnabled = false;
        }

        private void SetMinMax()
        {
            var px = _owner.PointsX;
            var py = _owner.PointsY;

            for (int i = 0; i < px.Count; ++i)
            {
                if (_min.X > px[i]) _min.X = px[i];
                if (_max.X < px[i]) _max.X = px[i];

                if (_min.Y > py[i]) _min.Y = py[i];
                if (_max.Y < py[i]) _max.Y = py[i];
            }
        }

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            
            if (_obstacle == null)
            {
                double w = double.Parse(Wide.Text.ToString()) / Constants.Sigma;
                double l = double.Parse(Length.Text.ToString()) / Constants.Sigma;

                var obstacle = new Obstacle(_owner) { UI = CreateNew(w, l) };
                _owner.Obstacles.Add(obstacle);
            } 
        }

        private Path CreateNew(double w, double l)
        {
            var pg = new PathGeometry { FillRule = FillRule.Nonzero };

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(_min.X + 5, _min.Y + 5);
            var startPoint = pf.StartPoint;

            for (int i = 0; i < 4; ++i)
            {
                var ls = new LineSegment();

                switch (i)
                {
                    case 0:
                        ls.Point = new Point(startPoint.X + w, startPoint.Y);
                        break;
                    case 1:
                        ls.Point = new Point(startPoint.X + w, startPoint.Y + l);
                        break;
                    case 2:
                        ls.Point = new Point(startPoint.X, startPoint.Y + l);
                        break;
                    case 3:
                        ls.Point = new Point(startPoint.X, startPoint.Y);
                        break;
                }
                pf.Segments.Add(ls);
            }

            var p = new Path { StrokeThickness = 2, Stroke = Colours.Black, Data = pg, Fill = Colours.Gray };

            return p;
        }

        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            var textBox = e.Source as TextBox;
            if (textBox == null) return;
            
            double d;
            var isParsed = double.TryParse(textBox.Text, out d);
            if (isParsed)
            {
                double w;
                double l;

                var parsedW = double.TryParse(Wide.Text, out w);
                var parsedL = double.TryParse(Length.Text, out l);
                
                if (!parsedL || !parsedW)
                {
                    BtnOK.IsEnabled = false;
                    return;
                }
                
                w /= Constants.Sigma;
                l /= Constants.Sigma;
                BtnOK.IsEnabled = (w < _max.X -1 && l < _max.Y - 1);
            }
            else
            {
                BtnOK.IsEnabled = false;
            }
        }
    }
}
