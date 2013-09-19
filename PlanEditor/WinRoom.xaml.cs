using System.Linq;
using MahApps.Metro.Controls;
using PlanEditor.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Helpers;

namespace PlanEditor
{
    public partial class WinRoom : Window
    {
        public Entity Entity { get; private set; }

        private readonly Place _place;
        private readonly Building _building;
        private readonly List<string> _lstType = new List<string>();
        private readonly List<List<string>> _subList = new List<List<string>>();
        private readonly List<string> _lst1 = new List<string>();
        private readonly List<string> _lst2 = new List<string>();
        private readonly List<string> _lst3 = new List<string>();
        private readonly List<string> _lst4 = new List<string>();
        private readonly List<string> _lst5 = new List<string>();
        private readonly List<string> _lst6 = new List<string>();
        private readonly List<string> _lst7 = new List<string>();
        private readonly Dictionary<object, bool> _fields = new Dictionary<object, bool>();
        private readonly Point _startPoint;
        private bool _isMore;
        private bool _isStairway;

        public WinRoom(Place place, Building building)
        {
            InitializeComponent();
            _place = place;
            _building = building;
            Title = _place.Name;

            if (!_place.IsMovable)
            {
                Wide.IsEnabled = false;
                Leng.IsEnabled = false;
            }

            InitializeCombo();
            Initialize();
        }

        public WinRoom(Point point, Building building)
        {
            InitializeComponent();
            InitializeCombo();
            Initialize();

            Title = "Новое помещение";

            Stairway.Visibility = Visibility.Hidden;
            _startPoint = point;
            _building = building;
        }

        private void InitializeCombo()
        {
            FillTypes();
            FillSubTypes();
            type.SelectionChanged += type_SelectionChanged;
        }

        private void Initialize()
        {
            if (_place == null)
            {
                _fields.Add(Wide, false);
                _fields.Add(Leng, false);
                _fields.Add(Height, false);
                _fields.Add(EvacWide, false);
                _fields.Add(StageFrom, true);
                _fields.Add(StageTo, true);
                _fields.Add(people, false);

                BtnOk.IsEnabled = false;
                return;
            }

            _fields.Add(Wide, true);
            _fields.Add(Leng, true);
            _fields.Add(Height, true);
            _fields.Add(EvacWide, true);
            _fields.Add(StageFrom, true);
            _fields.Add(StageTo, true);
            _fields.Add(people, true);
            
            var pg = _place.UI.Data as PathGeometry;
            if (pg == null) return;
            if (pg.Figures[0].Segments.Count > 4)
            {
                Wide.IsEnabled = false;
                Leng.IsEnabled = false;
                _isMore = true;
            }
            
            name.Text = _place.Name;
            people.Text = _place.Ppl.ToString();
            Height.Text = _place.Height.ToString();
            EvacWide.Text = _place.EvacWide.ToString();

            if (!_isMore)
            {
                Wide.Text = _place.Wide.ToString();
                Leng.Text = _place.Length.ToString();
            }            

            int selected = -1;
            if (_place.MainType != -1)
            {
                type.SelectedIndex = _place.MainType;

                if (type.SelectedIndex > -1 && type.SelectedIndex < 7) selected = type.SelectedIndex;
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

            if (type.SelectedIndex > -1 && type.SelectedIndex < 7) selected = type.SelectedIndex;

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

            Stairway.Visibility = type.SelectedIndex == 7 ? Visibility.Visible : Visibility.Hidden;

            if (_place != null)
            {
                _isStairway = (type.SelectedIndex == 7 && _place.Type != Entity.EntityType.Stairway);
                Stairway.Visibility = Visibility.Hidden;
                CheckFields();
            }
            else if (type.SelectedIndex == 7)
            {
                foreach (var field in _fields.Keys.ToArray())
                {
                    var textBox = field as TextBox;
                    if (textBox.Name.Contains("Stage"))
                    {
                        _fields[field] = false;
                    }
                }
                BtnOk.IsEnabled = false;
            }
    }
        
        private void Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            
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
            _lstType.Add("Спортивные помещения");
            _lstType.Add("Специализированные учебные аудитории");
            _lstType.Add("Учебно - вспомогательные помещения");
            _lstType.Add("Административные (офисные) помещения");
            _lstType.Add("Научно - исследовательское помещение");
            _lstType.Add("Служебные помещения");
            _lstType.Add("Лестничная клетка");                                                  // 7
            _lstType.Add("Жилые помещения гостиниц, общежитий  и т. д.");
            _lstType.Add("Столовая, буфет, зал ресторана");
            _lstType.Add("Зал театра, кинотеатра, клуба, цирка");
            _lstType.Add("Гардероб");
            _lstType.Add("Хранилища библиотек, архивы");
            _lstType.Add("Музеи, выставки");
            _lstType.Add("Подсобные и бытовые помещения");
            _lstType.Add("Административные помещения, учебные классы школ, ВУЗов, кабинеты поликлиник");
            _lstType.Add("Магазины");
            _lstType.Add("Зал вокзала");
            _lstType.Add("Стоянки легковых автомобилей");
            _lstType.Add("Стоянки легковых автомобилей (с двухуровневым хранением)");
            _lstType.Add("Стадионы");
            _lstType.Add("Спортзалы");
            _lstType.Add("Торговый зал гипермаркета");

            foreach (var item in _lstType.Select(t => new ComboBoxItem {Content = t}))
                type.Items.Add(item);
        }

        private void FillSubTypes()
        {
            _lst1.Add("Лекционная аудитория");
  	        _lst1.Add("Аудитория для практических занятий");
            _lst1.Add("Компьютерные классы (15 комп)");

            _lst2.Add("Большой спортивный зал, зал настольного тенниса");
	        _lst2.Add("Вне Университета");
	        _lst2.Add("Гимнастический зал");
	        _lst2.Add("Зал аэробики");
	        _lst2.Add("Раздевалка");
            _lst2.Add("Тренажёрный зал");

            _lst3.Add("Военная кафедра");
            _lst3.Add("Специализированная лаборатория");
            _lst3.Add("Специализированная мастерская");
            _lst3.Add("Мастерская рисунка, живописи, по проектированию");
	        _lst3.Add("Мастерская скульптуры");
            _lst3.Add("Мастерская по обработке материалов (дерево)");
            _lst3.Add("Мастерская по обработке материалов (металл)");
	        _lst3.Add("Сервисная мастерская");
	        _lst3.Add("Швейная мастерская");
	        _lst3.Add("Лингафонный кабинет");
            _lst3.Add("Телестудия");
            
            _lst4.Add("Актовый зал");
	        _lst4.Add("Зал заседаний");
	        _lst4.Add("Читальный зал");
	        _lst4.Add("Абонемент");
	        _lst4.Add("Фонд, архив");
            _lst4.Add("Музей");
            
            _lst5.Add("Кабинет руководителя");
            _lst5.Add("Кабинет сотрудника");
            _lst5.Add("Общая преподавательская");
            _lst5.Add("Помещение структурного подразделения");
            _lst5.Add("Приемная");
            _lst5.Add("Методический кабинет");
            _lst5.Add("Студенческий клуб");

            _lst6.Add("Аспирантская);");
            _lst6.Add("НИЛ");

            _lst7.Add("АТС");
	        _lst7.Add("Гараж (бокс)");
	        _lst7.Add("Гараж (бокс-ремонтный)");
	        _lst7.Add("Бойлерная");
	        _lst7.Add("Буфет");
            _lst7.Add("Вахта");
            _lst7.Add("Венткамера");
            _lst7.Add("Гардероб");
            _lst7.Add("Касса");
            _lst7.Add("Кинопроекторная");
            _lst7.Add("Коридор, холл");
            _lst7.Add("Котельная (нефть)");
            _lst7.Add("Лифт, лифтовая шахта");
            _lst7.Add("Медицинский пункт");
            _lst7.Add("Подсобное помещение");
            _lst7.Add("Подсобное помещение структурного подразделения");
            _lst7.Add("Производственное помещение");
            _lst7.Add("Санузел");
            _lst7.Add("Серверная");
            _lst7.Add("Склад (по типу назначения)");
            _lst7.Add("Столовая");
            _lst7.Add("Электрощитовая");
            _lst7.Add("Прочее");

            _subList.Add(_lst1);
            _subList.Add(_lst2);
            _subList.Add(_lst3);
            _subList.Add(_lst4);
            _subList.Add(_lst5);
            _subList.Add(_lst6);
            _subList.Add(_lst7);

        }

        private void EditPlace(double w, double l)
        {
            var pg = _place.UI.Data as PathGeometry;
            var startPoint = pg.Figures[0].StartPoint;

            int count = pg.Figures[0].Segments.Count;

            for (int i = 0; i < count; ++i)
            {
                var ls = pg.Figures[0].Segments[i] as LineSegment;
                var x = ls.Point.X;
                var y = ls.Point.Y;
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

            pf.StartPoint = new Point(_startPoint.X, _startPoint.Y);
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
            _place.Height = double.Parse(Height.Text.ToString());
            _place.EvacWide = double.Parse(EvacWide.Text.ToString());

            if (_place.Type != Entity.EntityType.Stairway)
            {
                _place.MainType = type.SelectedIndex;
                _place.SubType = subType.SelectedIndex;
            }
            else
            {
                var s = _place as Stairway;
                if (s == null) return;
                s.StageFrom = int.Parse(StageFrom.Text);
                s.StageTo = int.Parse(StageTo.Text);
            }

            if (_isMore && !_place.IsMovable) return;
            
            double wide = double.Parse(Wide.Text.ToString());
            double len = double.Parse(Leng.Text.ToString());

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

            if (type.SelectedIndex == 7)
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
                    UI = CreateNew(w, l),
                    EvacWide = double.Parse(EvacWide.Text),
                };

                if (type.SelectedIndex == 6 && subType.SelectedIndex == 10)
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
      
        private void Text_ChangedDouble(object sender, TextChangedEventArgs e)
        {
            var textBox = e.Source as TextBox;
            if (textBox == null) return;

            double d;
            double min = 1.0;
            var isParsed = double.TryParse(textBox.Text, out d);

            switch (textBox.Name)
            {
                case "EvacWide":
                    min = 0.5;
                    break;
            }
            _fields[textBox] = (isParsed && d > min);

            textBox.BorderBrush = _fields[textBox] ? Brushes.DarkGray : Brushes.Red;
            CheckFields();
        }

        private void Text_ChangedInt(object sender, TextChangedEventArgs e)
        {
            var textBox = e.Source as TextBox;
            if (textBox == null) return;

            int i;
            int min = 0;
            var isParsed = int.TryParse(textBox.Text, out i);
            
            if (textBox.Name == "people")
                min = 0;
            
            _fields[textBox] = type.SelectedIndex != 7 ? i >= min && isParsed : i >= 0 && isParsed;

            textBox.BorderBrush = _fields[textBox] ? Brushes.DarkGray : Brushes.Red;
            CheckFields();
        }

        private void Text_Stages(object sender, TextChangedEventArgs e)
        {
            var textBox = e.Source as TextBox;
            if (textBox == null) return;

            int stage = 0;
            int max = _building.Stages;
            
            var isParsed = int.TryParse(textBox.Text, out stage);
            
            _fields[textBox] = (isParsed && stage > 0 && stage <= max);
            textBox.BorderBrush = _fields[textBox] ? Brushes.DarkGray : Brushes.Red;

            CheckFields();
        }

        private void CheckFields()
        {
            var isOk = true;
            foreach (var field in _fields)
            {
                if (field.Value == false)
                {
                    isOk = false;
                    break;
                }
            }

            BtnOk.IsEnabled = (isOk && !_isStairway);
        }
    }
}


