using PlanEditor.Graph;
using PlanEditor.IO;
using PlanEditor.RegGrid;
using PlanEditor.SVGConverter;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlanEditor.Entities;

namespace PlanEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TransformGroup m_transformGroup;
        private TranslateTransform m_translation;
        private ScaleTransform m_scale;        
        private RegGrid.Grid m_grid;
        private int m_curStage = 0;                
        private double m_lastPosX;
        private double m_lastPosY;
        private Line m_line;
        private bool m_firstClick = false;
        private enum CanvasMode { Move, Path, Select, Connect }
        private CanvasMode Mode;
        private string m_fileName = "C:\\Users\\Andrey\\Desktop\\out";
        private bool m_ConnectClick1 = true;
        private bool m_ConnectClick2 = false;

        private List<Entity> m_selected = new List<Entity>();
        private List<Line> m_lines = new List<Line>();        
        private List<List<Line>> m_drawGrid = new List<List<Line>>();
        private List<Place> m_CurStagePlaces = new List<Place>();
        private List<Portal> m_CurStagePortals = new List<Portal>();

        Place place1 = null;
        Place place2 = null;

        private Entities.Building m_Building;

        public MainWindow()
        {
            InitializeComponent();
            
            m_transformGroup = new TransformGroup();
            m_translation = new TranslateTransform();
            m_scale = new ScaleTransform();
            m_transformGroup.Children.Add(m_translation);
            m_transformGroup.Children.Add(m_scale);

            m_Building = new Entities.Building();
            m_Building.Places.Add(m_CurStagePlaces);
            m_Building.Portals.Add(m_CurStagePortals);

            m_grid = new RegGrid.Grid(m_Building);            
        
            Mode = CanvasMode.Move;            

            #region MouseEvents

            ContentPanel.MouseMove += new MouseEventHandler(GridField_MouseMove);
            //ContentPanel.MouseRightButtonDown += new MouseButtonEventHandler(GridField_MouseRightButtonDown);
            ContentPanel.MouseLeftButtonUp += new MouseButtonEventHandler(GridField_MouseLeftButtonUp);
            ContentPanel.MouseLeftButtonDown += new MouseButtonEventHandler(GridField_MouseLeftButtonDown);
            ContentPanel.MouseDown += new MouseButtonEventHandler(GridField_MouseDown);
            ContentPanel.MouseLeave += new MouseEventHandler(GridField_MouseLeave);
            ContentPanel.MouseWheel += new MouseWheelEventHandler(GridField_MouseWheel);
            ContentPanel.KeyDown += new KeyEventHandler(GridField_KeyPressed);
            #endregion
        }

        private void GridField_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (Mode)
            { 
                case CanvasMode.Path:
                    if (Key.Escape == e.Key)
                    {
                        m_firstClick = true;
                        m_line = null;
                        m_lines.Clear();
                    }
                    break;
            }
        }
        
        private void GridField_MouseWheel(object sender, MouseEventArgs e)
        {
            
        }

        private void GridField_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        private void GridField_MouseDown(object sender, MouseEventArgs e)
        {
                     
        }

        /*private void GridField_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            
        }*/

        private void GridField_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(null).X;
            double y = e.GetPosition(null).Y;

            switch (Mode)
            {
                case CanvasMode.Path:                    
                    MovePath(x, y);
                    break;

                case CanvasMode.Move:
                    if (e.LeftButton == MouseButtonState.Pressed)
                        Move(x, y);
                    break;

                case CanvasMode.Select:
                    if (e.LeftButton == MouseButtonState.Pressed)
                        MoveSelected(x, y);
                    break;
            }
        }

        private void GridField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        

        private void GridField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(null).X - m_translation.X;
            double y = e.GetPosition(null).Y - m_translation.Y;

            x /= m_scale.ScaleX;
            y /= m_scale.ScaleY;

            if (e.ClickCount >= 2)
            {
                DeselectAll();
            }

            switch(Mode)
            {
                case CanvasMode.Path:                        
                    if (e.ClickCount >= 2)
                    {
                        MouseRBDownPath();
                    }
                    else
                    {
                        if (!m_firstClick)
                        {
                            m_firstClick = true;

                            AddNewLine(x, y);
                        }
                        else
                        {
                            m_line.X2 = x;
                            m_line.Y2 = y;

                            AddNewLine(x, y);
                        }
                    }
                break;

                case CanvasMode.Move:                    
                    m_lastPosX = e.GetPosition(null).X;
                    m_lastPosY = e.GetPosition(null).Y;
                    break;

                case CanvasMode.Select:
                    //test_DefineNode(e.GetPosition(null).X, e.GetPosition(null).Y);                    
                    m_lastPosX = e.GetPosition(null).X;
                    m_lastPosY = e.GetPosition(null).Y;
                    SelectObjects(x, y);
                    break;
                case CanvasMode.Connect:
                    ConnectPlaces(x, y);
                    break;
                default:
                    
                    break;
            }
        }
                  
        public void DrawGrid()
        {            
            for (int i = 0; i < m_Building.Stages; ++i)
            {
                var lines = new List<Line>();
                m_drawGrid.Add(lines);

                for (int m = 0; m < m_Building.Row + 1; ++m)
                {
                    Line l = new Line();
                    l.X1 = m * Data.GridStep;
                    l.Y1 = 0;
                    l.X2 = m * Data.GridStep;
                    l.Y2 = m_Building.yMax;

                    l.Stroke = Colours.LightGray;
                    l.StrokeThickness = 1;
                    l.RenderTransform = m_transformGroup;
                    ContentPanel.Children.Add(l);
                    
                    if (m_curStage != i)
                        l.Visibility = System.Windows.Visibility.Hidden;
                
                    lines.Add(l);                    
                }

                for (int n = 0; n < m_Building.Col + 1; ++n)
                {
                    Line l = new Line();
                    l.X1 = 0;
                    l.Y1 = n * Data.GridStep;
                    l.X2 = m_Building.xMax;
                    l.Y2 = n * Data.GridStep;

                    l.Stroke = Colours.LightGray;
                    l.StrokeThickness = 1;
                    l.RenderTransform = m_transformGroup;
                    ContentPanel.Children.Add(l);

                    if (m_curStage != i)
                        l.Visibility = System.Windows.Visibility.Hidden;

                    lines.Add(l);
                }                
            }            
        }

        private void Click_Open(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.DefaultExt = ".txt";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            //bool? result = dlg.ShowDialog();

            //if (result != null && result == true)
            //{
                ContentPanel.Children.Clear();
                m_Building = new Entities.Building();
                LoadFile lf = new LoadFile(m_fileName, m_Building);
                lf.Load();
                m_Building = lf.GetBuilding;
                m_curStage = 0;
                m_CurStagePlaces = m_Building.Places[m_curStage];
                m_CurStagePortals = m_Building.Portals[m_curStage];

                m_transformGroup = new TransformGroup();
                m_translation = new TranslateTransform();
                m_scale = new ScaleTransform();                
                m_transformGroup.Children.Add(m_translation);
                m_transformGroup.Children.Add(m_scale);

                for (int i = 0; i < m_Building.Stages; ++i)
                {
                    foreach (var v in m_Building.Places[i])
                    {
                        ContentPanel.Children.Add(v.UI);
                        v.UI.RenderTransform = m_transformGroup;
                        if (i != m_curStage)
                            v.UI.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                
                DrawPlan();                
            //}                
        }
               
        private void Click_Save(object sender, RoutedEventArgs e)
        {
            //SimpleGraph graph = ConverteGridToGraph.FromGraph(grid.Cells[0].Count);
            //SaveToEvac sv = new SaveToEvac();
            //sv.Save(@"C:\Users\Andrey\Desktop\out.txt", m_grid);

            SaveFile sf = new SaveFile(m_fileName, m_Building);
            sf.Save();
        }

        private void Click_Path(object sender, RoutedEventArgs e)
        {
            Mode = CanvasMode.Path;
            DeselectAll();
        }

        private void Click_SelectTool(object sender, RoutedEventArgs e)
        {
            Mode = CanvasMode.Select;
        }

        private void Click_MoveTool(object sender, RoutedEventArgs e)
        {
            Mode = CanvasMode.Move;
            DeselectAll();
        }
        
        private void Click_AddRoom(object sender, RoutedEventArgs e)
        {
            Place place = new Place();
            Room r = new Room(place);
            r.Owner = this;
            Nullable<bool> isOk = r.ShowDialog();

            if (isOk == null || isOk == false)
            {
                // remove all data
            }
            else if (isOk == true)
            {
                place.UI.RenderTransform = m_transformGroup;
                m_Building.Places[m_curStage].Add(place);
                ContentPanel.Children.Add(place.UI);
            }            
        }

        private void Click_AddDoor(object sender, RoutedEventArgs e)
        {
            Mode = CanvasMode.Connect;
        }
        
        private void Click_New(object sender, RoutedEventArgs e)
        {
            // Only for test
            LoadDefaultData();
            //Building b = new Building();
            //Nullable<bool> isOk = ShowDialog();
            
            DrawGrid();
            m_grid.CreateGrid();
        }

        private void Click_ZoomIn(object sender, RoutedEventArgs e)
        {
            m_scale.ScaleX /= 0.8;
            m_scale.ScaleY /= 0.8;
        }

        private void Click_ZoomOut(object sender, RoutedEventArgs e)
        {
            m_scale.ScaleX *= 0.8;
            m_scale.ScaleY *= 0.8;
        }

        private void Click_NextStage(object sender, RoutedEventArgs e)
        {
            if (m_curStage == m_Building.Stages - 1) return;
            
            SetHidden();
            ++m_curStage;
            SetVisible();
        }

        private void Click_PrevStage(object sender, RoutedEventArgs e)
        {
            if (m_curStage == 0) return;

            SetHidden();
            --m_curStage;
            SetVisible();
        }

        private void SetHidden()
        {
            foreach (var v in m_Building.Places[m_curStage])
            {
                v.UI.Visibility = System.Windows.Visibility.Hidden;
            }

            foreach (var v in m_Building.Portals[m_curStage])
            {
                v.UI.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SetVisible()
        {
            foreach (var v in m_Building.Places[m_curStage])
            {
                v.UI.Visibility = System.Windows.Visibility.Visible;
            }

            foreach (var v in m_Building.Portals[m_curStage])
            {
                v.UI.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void AddNewLine(double x, double y)
        {
            m_line = new Line();
            m_lines.Add(m_line);
            m_line.X1 = x;
            m_line.Y1 = y;

            m_line.Stroke = Colours.Black;
            m_line.StrokeThickness = 1;
            m_line.RenderTransform = m_transformGroup;

            ContentPanel.Children.Add(m_line);
        }

        private void DrawPlan()
        {
            foreach (var p in m_Building.Places[m_curStage])
            {
                p.UI.Visibility = System.Windows.Visibility.Visible;
            }

            foreach (var p in m_Building.Portals[m_curStage])
            {
                p.UI.Visibility = System.Windows.Visibility.Visible;
            }
        }

        //// Help functions
        private void MovePath(double x, double y)
        {
            if (!m_firstClick && m_line == null) return;

            double _x = x - m_translation.X;
            double _y = y - m_translation.Y;

            _x /= m_scale.ScaleX;
            _y /= m_scale.ScaleY;

            m_line.X2 = _x;
            m_line.Y2 = _y;
        }

        private void Move(double x, double y)
        {
            double d_x = MyMath.Geometry.Tan(x, m_lastPosX);
            double d_y = MyMath.Geometry.Tan(y, m_lastPosY);

            d_x /= m_scale.ScaleX;
            d_y /= m_scale.ScaleY;

            if (x > m_lastPosX)
                m_translation.X += d_x;
            else
                m_translation.X -= d_x;

            if (y > m_lastPosY)
                m_translation.Y += d_y;
            else
                m_translation.Y -= d_y;

            m_lastPosX = x;
            m_lastPosY = y;
        }

        private void MouseRBDownPath()
        {
            if (m_lines.Count == 0) return;

            m_line.X2 = m_lines[0].X1;
            m_line.Y2 = m_lines[0].Y1;

            var pg = new PathGeometry();
            pg.FillRule = FillRule.Nonzero;

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(m_lines[0].X1, m_lines[0].Y1);

            for (int i = 1; i < m_lines.Count; ++i)
            {
                LineSegment ls = new LineSegment();
                ls.Point = new Point(m_lines[i].X1, m_lines[i].Y1);

                pf.Segments.Add(ls);
            }

            LineSegment line = new LineSegment();
            line.Point = new Point(m_lines[0].X1, m_lines[0].Y1);
            pf.Segments.Add(line);

            foreach (var l in m_lines)
            {
                ContentPanel.Children.Remove(l);
            }

            Path p = new Path();
            p.Fill = Colours.Indigo;
            p.StrokeThickness = 2;
            p.Stroke = Colours.Black;
            p.Data = pg;
            p.RenderTransform = m_transformGroup;

            Place pl = new Place();
            pl.UI = p;            
            m_CurStagePlaces.Add(pl);
            
            ContentPanel.Children.Add(p);
                       
            m_firstClick = false;
            m_lines.Clear();
            m_line = null;
        }

        private void test_DefineNode(double x, double y)
        {
            x = x - m_translation.X;
            y = y - m_translation.Y;

            x /= m_scale.ScaleX;
            y /= m_scale.ScaleY;
            
            foreach (var c in m_grid.Cells[m_curStage])
            {
                double c_x = c.PosX + (Data.GridStep / 2);
                double c_y = c.PosY + (Data.GridStep / 2);

                double d = MyMath.Geometry.Tan(c_x, c_y, x, y);
                if (d < Data.GridStep/2)
                {
                    Rectangle r = new Rectangle();
                    Canvas.SetLeft(r, c.PosX);
                    Canvas.SetTop(r, c.PosY);
                    r.Width = Data.GridStep;
                    r.Height = Data.GridStep;

                    r.Fill = Colours.Green;

                    ContentPanel.Children.Add(r);
                }
            }
        }

        private void MoveSelected(double x, double y)
        {
            double d_x = MyMath.Geometry.Tan(x, m_lastPosX);
            double d_y = MyMath.Geometry.Tan(y, m_lastPosY);

            d_x /= m_scale.ScaleX;
            d_y /= m_scale.ScaleY;

            double moveX = 0;
            double moveY = 0;

            if (x > m_lastPosX)
                moveX += d_x;
            else
                moveX -= d_x;

            if (y > m_lastPosY)
                moveY += d_y;
            else
                moveY -= d_y;

            m_lastPosX = x;
            m_lastPosY = y;

            foreach (var v in m_selected)
            {
                if (v.Type == Entity.EntityType.Portal)
                {
                    Portal portal = v as Portal;
                    switch (portal.Orientation)
                    {
                        case Portal.PortalOrient.Horizontal:
                            if (x < portal.min || x > portal.max) return;
                            break;
                        case Portal.PortalOrient.Vertical:
                            if (y < portal.min || y > portal.max) return;
                            break;
                    }
                }                

                PathGeometry pg = v.UI.Data as PathGeometry;
                Point p = pg.Figures[0].StartPoint;
                double _x = p.X + moveX;
                double _y = p.Y + moveY;
                pg.Figures[0].StartPoint = new Point(_x, _y);

                int count = pg.Figures[0].Segments.Count;
                for (int i = 0; i < count; ++i)
                {
                    LineSegment ls = pg.Figures[0].Segments[i] as LineSegment;
                    _x = ls.Point.X + moveX;
                    _y = ls.Point.Y + moveY;
                    ls.Point = new Point(_x, _y);
                }                
            }
        }

        private void ConnectPlaces(double x, double y)
        {
            foreach (var p in m_Building.Places[m_curStage])
            {
                if (MyMath.Geometry.IsCollide(x, y, p.PointsX, p.PointsY))
                {
                    if (m_ConnectClick1)
                    {
                        m_ConnectClick1 = false;
                        m_ConnectClick2 = true;
                        place1 = p;
                    }
                    else if (m_ConnectClick2)
                    {
                        m_ConnectClick2 = false;
                        if (place1 == p) continue;
                        place2 = p;
                        ConnectTwoRooms();
                    }
                }
            }            
            //ConnectTwoRooms_d();
        }

        private void ConnectTwoRooms()
        {
            if (place1 != null)
                Debug.Write(place1.ID + " ");    
            else if (place2 != null)
                Debug.Write(place2.ID);

            Debug.WriteLine("\n");
        }

        private void Click_Property(object sender, RoutedEventArgs e)
        {
            if (m_selected.Count == 0) return;
            Entity entity = m_selected[0];
            if (entity.Type != Entity.EntityType.Place) return;

            Place curPlace =  entity as Place;         
            if (curPlace == null) return;

            Room r = new Room(curPlace);
            r.Owner = this;
            Nullable<bool> isOK = r.ShowDialog();
            
            if (isOK == null || isOK == false)
                return;
            if (isOK == true)
            { 
            
            }
        }

        private void SelectObjects(double x, double y)
        {
            foreach (var p in m_Building.Places[m_curStage])
            {
                if (MyMath.Geometry.IsCollide(x, y, p.PointsX, p.PointsY))
                {
                    if (m_selected.Contains(p))
                    {
                        m_selected.Remove(p);
                        p.Deselect();
                    } 
                    else
                    {
                        m_selected.Add(p);
                        p.Select();
                    }
                }
            }

            foreach (var p in m_Building.Portals[m_curStage])
            {
                if (MyMath.Geometry.IsCollide(x, y, p.PointsX, p.PointsY))
                {
                    if (m_selected.Contains(p))
                    {
                        m_selected.Remove(p);
                        p.Deselect();
                    }
                    else
                    {
                        m_selected.Add(p);
                        p.Select();
                    }
                }
            }
        }

        private void DeselectAll()
        {
            foreach (var v in m_selected)
                v.Deselect();

            m_selected.Clear();
        }

        private void Click_Remove(object sender, RoutedEventArgs e)
        {
            foreach (var v in m_selected)
            {
                if (v.Type != Entity.EntityType.Place) return;

                Place p = v as Place; 
                ContentPanel.Children.Remove(v.UI);
                m_Building.Places[m_curStage].Remove(p);
            }

            m_selected.Clear();
        }

        private void Click_AddMine(object sender, RoutedEventArgs e)
        {
            
        }

        private void Click_AddStage(object sender, RoutedEventArgs e)
        {
            SetHidden();
            
            ++m_curStage;
            ++m_Building.Stages;

            m_CurStagePlaces = new List<Place>();
            m_Building.Places.Add(m_CurStagePlaces);
            m_CurStagePortals = new List<Portal>();
            m_Building.Portals.Add(m_CurStagePortals);
            
            /*List<Place> mines = new List<Place>();
            foreach (var place in Data.Mines[m_curSage])
            { 
                mines   
            }
            ++m_curSage;
            */
        }
        private void LoadDefaultData()
        {
            m_Building.Lx = 10;
            m_Building.Ly = 11;
            m_Building.Stages = 1;

            m_Building.xMax = m_Building.Lx / Data.Sigma;
            m_Building.yMax = m_Building.Ly / Data.Sigma;

            m_Building.Row = (int)(m_Building.xMax / Data.GridStep);
            m_Building.Col = (int)(m_Building.yMax / Data.GridStep);
        }

        private void HideGrid()
        {
            for (int i = 0; i < m_Building.Stages; ++i)
                foreach (var v in m_drawGrid[i])
                    v.Visibility = System.Windows.Visibility.Hidden;
        }

        private void CollapseGrid()
        {
            for (int i = 0; i < m_Building.Stages; ++i)
                foreach (var v in m_drawGrid[i])
                    v.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Click_EditBuilding(object sender, RoutedEventArgs e)
        {
            Building bld = new Building(m_Building);
            bld.ShowDialog();
        }

        /*
        private void ConnectTwoRooms_d()
        {
            if (place1 != null && place2 != null)
            {
                Portal portal = new Portal();
                portal.RoomA = place1;
                portal.RoomB = place2;

                List<Line> lines1 = place1.Lines;
                List<Line> lines2 = place2.Lines;

                Line l1 = null;
                Line l2 = null;

                double d_x = MyMath.Geometry.Tan(m_ConnectClick1.X, m_ConnectClick2.X);
                double d_y = MyMath.Geometry.Tan(m_ConnectClick1.Y, m_ConnectClick2.Y);

                bool isVer = d_x > d_y ? true : false;
                if (isVer)
                    portal.Orientation = Portal.PortalOrient.Vertical;
                else
                    portal.Orientation = Portal.PortalOrient.Horizontal;
                
                double min = double.MaxValue;

                foreach (var l_1 in lines1)
                {
                    Point p1 = new Point(l_1.X1, l_1.Y1);
                    Point p2 = new Point(l_1.X2, l_1.Y2);

                    d_x = MyMath.Geometry.Tan(p1.X, p2.X);
                    d_y = MyMath.Geometry.Tan(p1.Y, p2.Y);

                    bool secVer = d_x > d_y ? true : false;

                    if (secVer == isVer) continue;
                    
                    foreach (var l_2 in lines2)
                    {
                        Point _p1 = new Point(l_2.X1, l_2.Y1);
                        Point _p2 = new Point(l_2.X2, l_2.Y2);

                        d_x = MyMath.Geometry.Tan(_p1.X, _p2.X);
                        d_y = MyMath.Geometry.Tan(_p1.Y, _p2.Y);

                        secVer = d_x < d_y ? true : false;
                        if (secVer != isVer) continue;

                        double v = -1;
                        if (isVer)
                            v = MyMath.Geometry.Tan(p1.X, _p1.X);
                        else
                            v = MyMath.Geometry.Tan(p1.Y, _p1.Y);

                        if (v < min && v != -1)
                        {
                            min = v;
                            l1 = l_1;
                            l2 = l_2;
                        }
                    }
                }
                double[] mas;

                if (isVer)
                    mas = GetMinMax(l1.Y1, l1.Y2, l2.Y1, l2.Y2);
                else
                    mas = GetMinMax(l1.X1, l1.X2, l2.X1, l2.X2);

                portal.min = mas[0];
                portal.max = mas[1];

                Debug.WriteLine(mas[0] + " " + mas[1]);

                m_CurStagePortals.Add(portal);

                double shift = 1.0 / Data.Sigma;

                //////////////////////////////////////////

                var pg = new PathGeometry();
                pg.FillRule = FillRule.Nonzero;

                var pf = new PathFigure();
                pg.Figures.Add(pf);

                pf.StartPoint = new Point(portal.min, l1.Y1);
                Point startPoint = pf.StartPoint;

                for (int i = 0; i < 4; ++i)
                {
                    LineSegment ls = new LineSegment();

                    switch (i)
                    {
                        case 0:
                            ls.Point = new Point(startPoint.X + shift, startPoint.Y);
                            break;
                        case 1:
                            ls.Point = new Point(startPoint.X + shift, startPoint.Y + 10);
                            break;
                        case 2:
                            ls.Point = new Point(startPoint.X, startPoint.Y + 10);
                            break;
                        case 3:
                            ls.Point = new Point(startPoint.X, startPoint.Y);
                            break;
                    }
                    pf.Segments.Add(ls);
                }

                Path p = new Path();
                p.RenderTransform = m_transformGroup;
                p.Fill = Colours.Red;
                p.StrokeThickness = 2;
                p.Stroke = Colours.Yellow;
                p.Data = pg;
                portal.UI = p;
                ContentPanel.Children.Add(p);
                /////////////////////////////
                             
                place1 = null;
                place2 = null;
            }
        }
        // переделать!!!!
        private double[] GetMinMax(double p1, double p2, double p3, double p4)
        {
            double[] mas = { p1, p2, p3, p4};
            double max = double.MinValue;
            double min = double.MaxValue;

            for (int i = 0; i < mas.Length; ++i)
            {
                if (mas[i] > max)
                    max = mas[i];
                else if (mas[i] < min)
                    min = mas[i];
            }

            double a = -1;
            double b = -1;
            for (int i = 0; i < mas.Length; ++i)
            {
                if (mas[i] == max || mas[i] == min) continue;
                
                if (a == -1)
                    a = mas[i];
                else if (b == -1)
                    b = mas[i];
            }
            Debug.WriteLine("min: " + min + " max: " + max + " a: " + a + " b: " + b);
            if (a < b)
                return new double[] { a, b };
            else
                return new double[] { b, a };
        }
        */
        private void Click_Grid(object sender, RoutedEventArgs e)
        {
            RecognizeGrid rg = new RecognizeGrid(m_grid, m_Building);
            rg.Recognize();

            /*for (int n = 0; n < m_Building.Col; ++n)
            {
                for (int m = 0; m < m_Building.Row; ++m)
                {
                    foreach (var v in m_grid.Cells[m_curStage])
                    {
                        if (v.M == m && v.N == n)
                        {
                            Debug.Write(v.ID + "(" + v.Owner + ") ");
                            break;
                        }
                    }
                }
                Debug.WriteLine("\n");
            }//*/
        }

        private void Click_Export(object sender, RoutedEventArgs e)
        {
            SaveToEvac sv = new SaveToEvac();
            sv.Save("C:\\Users\\Andrey\\Desktop\\out.txt", m_grid, m_Building);
        }
    }    
}
