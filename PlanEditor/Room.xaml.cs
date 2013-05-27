using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PlanEditor.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Helpers;

namespace PlanEditor
{
    public partial class Room : Window
    {
        public Entity Entity { get; private set; }

        private Place _place;
        private List<Place> _places;
        private List<string> _lstType = new List<string>();
        private List<List<string>> _subList = new List<List<string>>();
        private List<string> _lst2 = new List<string>();
        private List<string> _lst3 = new List<string>();
        private List<string> _lst4 = new List<string>();
        private List<string> _lst6 = new List<string>();
        private bool _isMore = false;

        public Room(Place place)
        {
            InitializeComponent();
            _place = place;

            Title = _place.Name;

            InitializeCombo();
            Initialize();
        }

       

        public Room(List<Place> places)
        {
            InitializeComponent();
            InitializeCombo();

            Title = "Помещения";

            name.IsEnabled = false;
            Wide.IsEnabled = false;
            Height.IsEnabled = false;
            Leng.IsEnabled = false;

            Stairway.Visibility = Visibility.Hidden;

            _places = places;
        }   

        public Room()
        {
            InitializeComponent();
            InitializeCombo();

            Title = "Новое помещение";

            Stairway.Visibility = Visibility.Hidden;
        }

        private void InitializeCombo()
        {
            FillTypes();
            FillSubTypes();
            type.SelectionChanged += type_SelectionChanged;
        }

        private void Initialize()
        {            

            if (_place.UI != null)
            {
                var pg = _place.UI.Data as PathGeometry;
                if (pg.Figures[0].Segments.Count > 4)
                {
                    Wide.IsEnabled = false;
                    Leng.IsEnabled = false;
                    _isMore = true;
                }
            }           
            
            name.Text = _place.Name;
            people.Text = _place.Ppl.ToString(CultureInfo.InvariantCulture);
            Height.Text = _place.Height.ToString(CultureInfo.InvariantCulture);

            if (!_isMore)
            {
                Wide.Text = _place.Wide.ToString(CultureInfo.InvariantCulture);
                Leng.Text = _place.Length.ToString(CultureInfo.InvariantCulture);
            }            

            int selected = -1;
            if (_place.MainType != -1)
            {
                type.SelectedIndex = _place.MainType;

                switch (_place.MainType)
                {
                    case 1:
                        selected = 0;
                        break;
                    case 2:
                        selected = 1;
                        break;
                    case 3:
                        selected = 2;
                        break;
                    case 5:
                        selected = 3;
                        break;
                }
            }

            if (selected != -1)
            {
                subType.IsEnabled = true;
                subType.Items.Clear();
                foreach (var v in _subList[selected])
                {
                    var item = new ComboBoxItem { Content = v };
                    subType.Items.Add(item);
                }

                subType.SelectedIndex = _place.SubType;
            }
            else
            { 
                subType.IsEnabled = false;
            }

            if (_place.Type == Entity.EntityType.Stairway)
            {
                Stairway.Visibility = Visibility.Visible;
                type.IsEnabled = false;
                subType.IsEnabled = false;
                var s = _place as Stairway;
                if (s != null)
                {
                    StageFrom.Text = s.StageFrom.ToString();
                    StageTo.Text = s.StageTo.ToString();
                }
            }
            else 
            {
                Stairway.Visibility = Visibility.Hidden;
            }
            
        }

        private void type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            subType.Items.Clear();
            int selected = -1;
            
            switch (type.SelectedIndex)
            { 
                case 1:
                    selected = 0;
                    break;
                case 2:
                    selected = 1;
                    break;
                case 3:
                    selected = 2;
                    break;
                case 5:
                    selected = 3;
                    break;
            }

            if (selected == -1)
            {
                subType.IsEnabled = false;
            }
            else
            {
                subType.IsEnabled = true;
                foreach (var v in _subList[selected])
                {
                    var item = new ComboBoxItem {Content = v};
                    subType.Items.Add(item);
                }
            }

            Stairway.Visibility = type.SelectedIndex == type.Items.Count - 1 ? Visibility.Visible : Visibility.Hidden;
        }
               

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            if (_places != null && _place == null)
            {
                Places();
                return;
            }

            if (_place != null)
            {
                PlaceNotNull();
            } 
            else
            {
                PlaceNull();
            }
        }

        private void FillTypes()
        {
            _lstType.Add("Учебные аудитории");
            _lstType.Add("Специализированные помещения");
            _lstType.Add("Спортивные помещения");
            _lstType.Add("Раздевалка (молодежная одежда)");
            _lstType.Add("Помещения специализированных лабораторий");
            _lstType.Add("Помещения залов");
            _lstType.Add("Зал книг");
            _lstType.Add("Препараторская, медицинский пункт");
            _lstType.Add("Офисные помещения");
            _lstType.Add("Научно-исследовательское помещение ");
            _lstType.Add("Помещения с техникой");
            _lstType.Add("Бойлерная,  венткамера");
            _lstType.Add("Столовая, буфет, зал ресторана");
            _lstType.Add("Коридор, холл, санузел");
            _lstType.Add("Котельная (нефть)");
            _lstType.Add("Подсобное помещение");
            _lstType.Add("Бытовые помещения");
            _lstType.Add("Производственное помещение");
            _lstType.Add("Склад (по типу назначения)");
            _lstType.Add("Магазины");
            _lstType.Add("Стоянки легковых a\\м");
            _lstType.Add("Стоянки легковых а\\м (с двухуровневым хранением)");
            _lstType.Add("Лестница");

            foreach (var item in _lstType.Select(t => new ComboBoxItem {Content = t}))
                type.Items.Add(item);
        }

        private void FillSubTypes()
        {
            _lst2.Add("Серверная");
            _lst2.Add("Лингафонный кабинет");

            _lst3.Add("Большие спортзалы");
            _lst3.Add("Стадионы");

            _lst4.Add("Гардероб");

            _lst6.Add("Жилые помещения гостиниц, общежитий");

            _subList.Add(_lst2);
            _subList.Add(_lst3);
            _subList.Add(_lst4);
            _subList.Add(_lst6);
        }

        private void EditPlace(double w, double l)
        {
            var pg = _place.UI.Data as PathGeometry;
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
                        y = startPoint.Y + l;
                        break;
                    case 2:
                    //case 5:
                        y = startPoint.Y + l;
                        break;
                }

                ls.Point = new Point(x, y);
            }
        }

        private Path CreateNew(double w, double l)
        {
            var pg = new PathGeometry {FillRule = FillRule.Nonzero};

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(100, 100);
            var startPoint = pf.StartPoint;

            for (int i = 0; i < 4; ++i)
            {
                var ls = new LineSegment();

                switch (i)
                {
                    case 0:
                    //case 1:
                        ls.Point = new Point(startPoint.X + w, startPoint.Y);                        
                        break;
                    case 1:
                   // case 3:
                        ls.Point = new Point(startPoint.X + w, startPoint.Y + l);
                        break;
                    case 2:
                    //case 5:
                        ls.Point = new Point(startPoint.X, startPoint.Y + l);
                        break;
                    case 3:
                        ls.Point = new Point(startPoint.X, startPoint.Y);
                        break;
                }
                pf.Segments.Add(ls);
            }

            var p = new Path {StrokeThickness = 2, Stroke = Colours.Black, Data = pg};

            return p;
        }
    
        private void PlaceNotNull()
        {
            _place.Name = name.Text;
            _place.Ppl = int.Parse(people.Text);
            _place.Height = double.Parse(Height.Text);
            if (_place.Type != Entity.EntityType.Stairway)
            {
                _place.MainType = type.SelectedIndex;
                _place.SubType = subType.SelectedIndex;
            }
            else
            {
                var s = _place as Stairway;
                s.StageFrom = int.Parse(StageFrom.Text);
                s.StageTo = int.Parse(StageTo.Text);
            }


            if (_isMore) return;

            double wide = double.Parse(Wide.Text);
            double len = double.Parse(Leng.Text);

            double w = wide / Data.Sigma;
            double l = len / Data.Sigma;

            var pg = _place.UI.Data as PathGeometry;
            if (pg != null && pg.Figures[0].Segments.Count < 5)
            {
                EditPlace(w, l);
            }
        }
       
        private void PlaceNull()
        {
            double wide = double.Parse(Wide.Text);
            double len = double.Parse(Leng.Text);

            double w = wide / Data.Sigma;
            double l = len / Data.Sigma;

            if (type.SelectedIndex == type.Items.Count - 1)
            {
                var stairway = new Stairway
                {
                    Name = name.Text,
                    Ppl = int.Parse(people.Text),
                    Height = double.Parse(Height.Text),
                    StageFrom = int.Parse(StageFrom.Text),
                    StageTo = int.Parse(StageTo.Text),
                    UI = CreateNew(w, l)
                };

                stairway.UI.Fill = Colours.Violet;
                Entity = stairway;
            }
            else
            {
                var p = new Place
                {
                    Name = name.Text,
                    Ppl = int.Parse(people.Text),
                    Height = double.Parse(Height.Text),
                    MainType = type.SelectedIndex,
                    SubType = subType.SelectedIndex,
                    UI = CreateNew(w, l)
                };

                if (type.SelectedIndex == 13)
                {
                    p.UI.Fill = Colours.Green;
                    p.Type = Entity.EntityType.Halfway;
                }
                else
                {
                    p.UI.Fill = Colours.Indigo;
                }
                Entity = p;
            }
        }
    
        private void Places()
        {
            foreach (var plc in _places)
            {
                plc.MainType = type.SelectedIndex;
                plc.SubType = subType.SelectedIndex;
                plc.Ppl = int.Parse(people.Text);
            }    
        }

        private void Text_ChangedDouble(object sender, TextChangedEventArgs e)
        {
            var textBox = e.Source as TextBox;
            if (textBox == null) return;
            double i;
            BtnOk.IsEnabled = double.TryParse(textBox.Text, out i);
        }

        private void Text_ChangedPeople(object sender, TextChangedEventArgs e)
        {
            int d;
            BtnOk.IsEnabled = int.TryParse(people.Text, out d);
        }
    }
}
