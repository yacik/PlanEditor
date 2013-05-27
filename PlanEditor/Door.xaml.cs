using System.Windows;
using System.Windows.Media;
using PlanEditor.Entities;
using PlanEditor.Helpers;

namespace PlanEditor
{
    /// <summary>
    /// Interaction logic for Door.xaml
    /// </summary>
    public partial class Door : Window
    {
        public double Wide { get; private set; }
        private readonly Portal _portal;

        public Door()
        {
            Wide = -1;
            InitializeComponent();

            Title = "Добавить дверь";
        }

        public Door(Portal portal)
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
            else
            {
                
            }
        }
        private void Text_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            double d;
            btnOk.IsEnabled = double.TryParse(WideText.Text, out d);
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
