using PlanEditor.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PlanEditor
{
    public partial class Room : Window
    {
        private Place place;

        bool isMore = false;
        bool isNew = true;
        private List<string> m_lstType = new List<string>();
        private List<List<string>> m_subList = new List<List<string>>();
        private List<string> m_lst2 = new List<string>();
        private List<string> m_lst3 = new List<string>();
        private List<string> m_lst4 = new List<string>();        
        private List<string> m_lst6 = new List<string>();
        
        public Room(Place place)
        {
            InitializeComponent();
            this.place = place;
            Initialize();
        }

        private void Initialize()
        {
            FillTypes();
            FillSubTypes();
            type.SelectionChanged += type_SelectionChanged;

            if (place.UI != null)
            {
                isNew = false;
                PathGeometry pg = place.UI.Data as PathGeometry;
                if (pg.Figures[0].Segments.Count > 4)
                {
                    Wide.IsEnabled = false;
                    Leng.IsEnabled = false;
                    isMore = true;
                }
            }           
            
            name.Text = place.Name;
            people.Text = place.Ppl.ToString();                
            Height.Text = place.Height.ToString();

            if (!isMore && !isNew)
            {
                Wide.Text = place.Wide.ToString();
                Leng.Text = place.Length.ToString();
            }            

            int selected = -1;
            if (place.MainType != -1)
            {
                type.SelectedIndex = place.MainType;

                switch (place.MainType)
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
                foreach (var v in m_subList[selected])
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = v;
                    subType.Items.Add(v);
                }

                subType.SelectedIndex = place.SubType;
            }
            else
            { 
                subType.IsEnabled = false;
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
                foreach (var v in m_subList[selected])
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = v;
                    subType.Items.Add(v);
                }
            }
        }
               

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            place.Name = name.Text;
            place.Ppl = int.Parse(people.Text);    
            place.Height = double.Parse(Height.Text);
            place.MainType = type.SelectedIndex;
            place.SubType = subType.SelectedIndex;


            if (isMore)
                return;

            double _wide = double.Parse(Wide.Text);
            double _len = double.Parse(Leng.Text);
            
            double w = _wide / Data.Sigma;
            double l = _len / Data.Sigma;

            if (isNew)
            {
                CreateNew(w, l);
            }
            else
            {                
                PathGeometry pg = place.UI.Data as PathGeometry;
                if (pg.Figures[0].Segments.Count < 5)
                {
                    EditPlace(w, l);
                }
            }
        }

        private void FillTypes()
        {
            m_lstType.Add("Учебные аудитории");
            m_lstType.Add("Специализированные помещения");
            m_lstType.Add("Спортивные помещения");
            m_lstType.Add("Раздевалка (молодежная одежда)");
            m_lstType.Add("Помещения специализированных лабораторий");
            m_lstType.Add("Помещения залов");
            m_lstType.Add("Зал книг");
            m_lstType.Add("Препараторская, медицинский пункт");
            m_lstType.Add("Офисные помещения");
            m_lstType.Add("Научно-исследовательское помещение ");
            m_lstType.Add("Помещения с техникой");
            m_lstType.Add("Бойлерная,  венткамера");
            m_lstType.Add("Столовая, буфет, зал ресторана");
            m_lstType.Add("Коридор, лестничная клетка, холл, санузел");
            m_lstType.Add("Котельная (нефть)");
            m_lstType.Add("Подсобное помещение");
            m_lstType.Add("Бытовые помещения");
            m_lstType.Add("Производственное помещение");
            m_lstType.Add("Склад (по типу назначения)");
            m_lstType.Add("Магазины");
            m_lstType.Add("Стоянки легковых a\\м");
            m_lstType.Add("Стоянки легковых а\\м (с двухуровневым хранением)");

            for (int i = 0; i < m_lstType.Count; ++i)
            {
                var item = new ComboBoxItem();
                item.Content = m_lstType[i];
                type.Items.Add(item);
            }
        }

        private void FillSubTypes()
        {
            m_lst2.Add("Серверная");
            m_lst2.Add("Лингафонный кабинет");

            m_lst3.Add("Большие спортзалы");
            m_lst3.Add("Стадионы");

            m_lst4.Add("Гардероб");

            m_lst6.Add("Жилые помещения гостиниц, общежитий");

            m_subList.Add(m_lst2);
            m_subList.Add(m_lst3);
            m_subList.Add(m_lst4);
            m_subList.Add(m_lst6);
        }

        private void EditPlace(double w, double l)
        {
            PathGeometry pg = place.UI.Data as PathGeometry;
            Point startPoint = pg.Figures[0].StartPoint;

            int count = pg.Figures[0].Segments.Count;

            for (int i = 0; i < count; ++i)
            {
                LineSegment ls = pg.Figures[0].Segments[i] as LineSegment;
                double _x = ls.Point.X;
                double _y = ls.Point.Y;
                switch (i)
                {
                    case 0:
                    //case 1:
                        _x = startPoint.X + w;
                        break;
                    case 1:
                    //case 3:
                        _x = startPoint.X + w;
                        _y = startPoint.Y + l;
                        break;
                    case 2:
                    //case 5:
                        _y = startPoint.Y + l;
                        break;
                }

                ls.Point = new Point(_x, _y);
            }
        }

        private void CreateNew(double w, double l)
        {
            var pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(100, 100);
            Point startPoint = pf.StartPoint;

            for (int i = 0; i < 4; ++i)
            {
                LineSegment ls = new LineSegment();

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

            Path p = new Path();
            p.Fill = Colours.Indigo;
            p.StrokeThickness = 2;
            p.Stroke = Colours.Black;
            p.Data = pg;
            place.UI = p;
        }
    }
}
