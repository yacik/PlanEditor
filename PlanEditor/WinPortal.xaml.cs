using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlanEditor.Entities;
using PlanEditor.Helpers;

namespace PlanEditor
{
    public partial class WinPortal : Window
    {
        public double Wide { get; private set; }
        private readonly Portal _portal;

        public WinPortal()
        {
            Wide = -1;
            InitializeComponent();

            Title = "Добавить дверь";

            btnOk.IsEnabled = false;
        }

        public WinPortal(Portal portal)
        {
            InitializeComponent();
            
            WideText.Text = portal.Wide.ToString();

            Title = "Редактирование двери";

            _portal = portal;
        }


        private void Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            if (_portal == null)
            {
                Wide = double.Parse(WideText.Text);
            }
            else
            {
                Wide = double.Parse(WideText.Text);
                _portal.Width = Wide;
                EditPlace();
            }
        }
        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            double d;
            var isParsed = double.TryParse(WideText.Text, out d);
            
            if (_portal == null)
            {
                if (d > 0 && isParsed)
                    btnOk.IsEnabled = true;
                else
                    btnOk.IsEnabled = false;
            }
            else
            {
                double metr = (_portal.Max - _portal.Min) * Data.Sigma;
                if (d > 0 && isParsed && d <= metr)
                    btnOk.IsEnabled = true;
                else
                    btnOk.IsEnabled = false;
            }
        }

        private void EditPlace()
        {
            double w = 0.0;
            double l = 0.0;

            if (_portal.Orientation == Portal.PortalOrient.Vertical)
                l = Wide / Data.Sigma;
            else
                w = Wide / Data.Sigma;

            var pg = _portal.UI.Data as PathGeometry;
            var startPoint = pg.Figures[0].StartPoint;

            int count = pg.Figures[0].Segments.Count;

            for (int i = 0; i < count; ++i)
            {
                var ls = (LineSegment)pg.Figures[0].Segments[i];
                var x = ls.Point.X;
                var y = ls.Point.Y;

                if (_portal.Orientation == Portal.PortalOrient.Horizontal)
                {
                    switch (i)
                    {
                        case 0:
                            x = startPoint.X + w;
                            break;
                        case 1:
                            x = startPoint.X + w;
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 1:
                            y = startPoint.Y + l;
                            break;
                        case 2:
                            y = startPoint.Y + l;
                            break;
                    }
                }
                
                ls.Point = new Point(x, y);
            }
        }
    }
}
