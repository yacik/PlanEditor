using System;
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
            InitializeComponent();

            Wide = -1;
            HeightText.Text = "1.9";
            
            Title = "Добавить дверь";

            btnOk.IsEnabled = false;

            WideText.Text = TempData.TempDoor.Wide;
            HeightText.Text = TempData.TempDoor.Height;
            DepthText.Text = TempData.TempDoor.Depth;
        }

        public WinPortal(Portal portal)
        {
            InitializeComponent();
            
            Title = "Редактирование двери";

            _portal = portal;
           
            HeightText.Text = _portal.Height.ToString();
            WideText.Text = _portal.Width.ToString();
            DepthText.Text = _portal.Depth.ToString();
        }


        private void Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            TempData.TempDoor.Wide = WideText.Text;
            TempData.TempDoor.Height = HeightText.Text;
            TempData.TempDoor.Depth = DepthText.Text;

            if (_portal == null)
            {
                Wide = double.Parse(WideText.Text);
            }
            else
            {
                Wide = double.Parse(WideText.Text);
                _portal.Width = Wide;

                try
                {
                    _portal.Height = double.Parse(HeightText.Text);
                    _portal.Depth = double.Parse(DepthText.Text);

                    EditPlace();
                } 
                catch(Exception ex)
                {
                    PELogger.GetLogger.WriteLn(ex.Message);
                }
            }

            
        }
        private void Text_Changed(object sender, TextChangedEventArgs e)
        {
            double d;
            var isParsed = double.TryParse(WideText.Text, out d);
            
            if (_portal == null)
            {
                btnOk.IsEnabled = (d > 0 && isParsed);
            }
            else
            {
                double metr = (_portal.Max - _portal.Min) * Constants.Sigma;
                btnOk.IsEnabled = (d > 0 && isParsed && d <= metr);
            }
        }

        private void EditPlace()
        {
            double w = 0.0;
            double l = 0.0;

            if (_portal.Orientation == Portal.PortalOrient.Vertical) l = Wide / Constants.Sigma;
            else w = Wide / Constants.Sigma;

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
