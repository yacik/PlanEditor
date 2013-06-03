using System.Windows;
using System.Windows.Media;
using PlanEditor.Entities;
using PlanEditor.Helpers;

namespace PlanEditor
{
    /// <summary>
    /// Interaction logic for WinPortal.xaml
    /// </summary>
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
            if (_portal == null)
            {
                DialogResult = true;
                Wide = double.Parse(WideText.Text);
            }
        }
        private void Text_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            double d;
            var isParsed = double.TryParse(WideText.Text, out d);

            if (d > 0 && isParsed)
                btnOk.IsEnabled = true;
            else
                btnOk.IsEnabled = false;
        }

        private void EditPlace()
        {
            double w = Wide / Data.Sigma;

            var pg = _portal.UI.Data as PathGeometry;
            var startPoint = pg.Figures[0].StartPoint;

            int count = pg.Figures[0].Segments.Count;

            for (int i = 0; i < count; ++i)
            {
                var ls = pg.Figures[0].Segments[i] as LineSegment;
                double x = ls.Point.X;
                double y = ls.Point.Y;
                switch (i)
                {
                    case 0:
                        //case 1:
                        x = startPoint.X + w;
                        break;
                    case 1:
                        //case 3:
                        x = startPoint.X + w;
                        y = startPoint.Y;
                        break;
                    case 2:
                        //case 5:
                        y = startPoint.Y;
                        break;
                }

                ls.Point = new Point(x, y);
            }
        }
    }
}
