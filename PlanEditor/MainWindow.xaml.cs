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
    public partial class MainWindow : Window
    {
        private enum CanvasMode { Move, Path, Select, Connect }
        private CanvasMode Mode;

        private TransformGroup m_transformGroup;
        private TranslateTransform m_translation;
        private ScaleTransform m_scale;        
        private RegGrid.Grid m_grid;
        private int m_curStage = 0;                
        private double m_lastPosX;
        private double m_lastPosY;
        private Line m_line;
        private Entities.Building m_Building;
        private bool m_firstClick = false;        
        private string m_fileName = "C:\\Users\\Andrey\\Desktop\\out";
        private bool m_isConnected = true;
        private Place m_place1 = null;
        private Place m_place2 = null;
        private Point m_firstClickPlace;
        private Point m_secondClickPlace;
        private List<Entity> m_selected = new List<Entity>();
        private List<Line> m_lines = new List<Line>();        
        private List<List<Line>> m_drawGrid = new List<List<Line>>();
        private List<Place> m_CurStagePlaces = new List<Place>();
        private List<Portal> m_CurStagePortals = new List<Portal>();        
        
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

            m_line.X1 = x;
            m_line.Y1 = y;
            m_line.X2 = x;
            m_line.Y2 = y;

            m_line.Stroke = Colours.Black;
            m_line.StrokeThickness = 1;
            m_line.RenderTransform = m_transformGroup;
            
            ContentPanel.Children.Add(m_line);
            m_lines.Add(m_line);
        }

        private void MovePath(double x, double y)
        {
            if (!m_firstClick && m_line == null) return;

            x = x - m_translation.X;
            y = y - m_translation.Y;

            x /= m_scale.ScaleX;
            y /= m_scale.ScaleY;

            m_line.X2 = x;
            m_line.Y2 = y;
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
                switch(v.Type)
                {
                    case Entity.EntityType.Portal:
                        MovePortal(x, y, moveX, moveY, v);
                        break;
                    case Entity.EntityType.Place:
                        MovePlace(x, y, moveX, moveY, v);
                        break;

                    default:
                    break;
                }
            }
        }

        private void ConnectPlaces(double x, double y)        
        {
            if (m_isConnected)
            {
                m_isConnected = !m_isConnected;
                m_firstClickPlace = new Point(x, y);

                foreach (var p in m_Building.Places[m_curStage])
                {
                    if (MyMath.Geometry.IsCollide(x, y, p.PointsX, p.PointsY))
                    {
                        m_place1 = p;
                        break;
                    }
                }
            }
            else if (!m_isConnected)
            {
                m_isConnected = !m_isConnected;
                m_secondClickPlace = new Point(x, y);

                foreach (var p in m_Building.Places[m_curStage])
                {
                    if (MyMath.Geometry.IsCollide(x, y, p.PointsX, p.PointsY))
                    {
                        if (m_place1 != p)
                        {
                            m_place2 = p;
                            break;
                        }
                    }
                }
                m_lastPosX = x;
                m_lastPosY = y;

                if (m_place1 == null && m_place2 == null)
                {
                    MessageBox.Show("Помещения не выбраны");
                    return;
                }

                if (m_place1 == null || m_place2 == null)
                    ConnectOutsidePortal();
                if (m_place1 != null && m_place2 != null)
                    ConncetInnerPortal();                
            }
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

        private void ConnectOutsidePortal()
        {            
            Place place = null;

            if (m_place1 == null)
                place = m_place2;
            else
                place = m_place1;

            Portal portal = new Portal();
            portal.RoomA = place;
            portal.RoomB = null;
            m_Building.Portals[m_curStage].Add(portal);
            portal.RoomA.IsMovable = false;

            double min = double.MaxValue;
            List<Line> lines = place.Lines;
            
            foreach (var v in lines)
            {
                double d = 0;
                
                if (IsHorizontal(v.X1, v.Y1, v.X2, v.Y2))
                    d = MyMath.Geometry.Tan(m_lastPosY, v.Y1);                    
                else
                    d = MyMath.Geometry.Tan(m_lastPosX, v.X1);

                if (d < min)
                {
                    min = d;
                    portal.ParentWall = v;
                }
            }
            Line line = portal.ParentWall;
            if (IsHorizontal(line.X1, line.Y1, line.X2, line.Y2))
            {
                portal.Orientation = Portal.PortalOrient.Horizontal;                
                portal.SetUI();                
            }
            else
            {
                portal.Orientation = Portal.PortalOrient.Vertical;
                portal.SetUI();                
            }

            portal.UI.RenderTransform = m_transformGroup;
            ContentPanel.Children.Add(portal.UI);

            m_place1 = null;
            m_place2 = null;
        }

        private void ConncetInnerPortal()
        {
            Portal portal = new Portal();
            portal.RoomA = m_place1;
            portal.RoomB = m_place2;
            portal.RoomA.IsMovable = false;
            portal.RoomB.IsMovable = false;

            m_Building.Portals[m_curStage].Add(portal);

            List<Line> lines1 = m_place1.Lines;
            List<Line> lines2 = m_place2.Lines;

            Line line1 = null;
            Line line2 = null;

            foreach (var l1 in lines1)
            {
                double min = double.MaxValue;

                bool isHor1 = IsHorizontal(l1.X1, l1.Y1, l1.X2, l1.Y2);

                foreach (var l2 in lines2)
                {
                    bool isHor2 = IsHorizontal(l2.X1, l2.Y1, l2.X2, l2.Y2);

                    if (isHor1 && isHor2) 
                    {
                        double d = MyMath.Geometry.Tan(l1.Y1, l2.Y1);
                        if (d < min)
                        {
                            min = d;
                            line1 = l1;
                            line2 = l2;
                        }
                    }
                    else if (!isHor1 && !isHor2)
                    {                     
                        double d = MyMath.Geometry.Tan(l1.X1, l2.X1);
                        if (d < min)
                        {
                            min = d;
                            line1 = l1;
                            line2 = l2;
                        }
                    }
                }
            }

            line1.Stroke = Colours.Emerland;
            line2.Stroke = Colours.Emerland;
            line1.StrokeThickness = 3;
            line2.StrokeThickness = 3;
            ContentPanel.Children.Add(line1);
            ContentPanel.Children.Add(line2);
        }

        private bool IsHorizontal(double x1, double y1, double x2, double y2)
        {
            double x = MyMath.Geometry.Tan(x1, x2);
            double y = MyMath.Geometry.Tan(y1, y2);

            return (x > y);
        }

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
        private void MovePortal(double x, double y, double moveX, double moveY, Entity v)
        {
            Portal portal = v as Portal;
            PathGeometry pg = v.UI.Data as PathGeometry;
            Point p = pg.Figures[0].StartPoint;
            double sX = p.X;
            double sY = p.Y;

            if (portal.Orientation == Portal.PortalOrient.Horizontal)
            {                
                sX = p.X + moveX;
                sY = p.Y;
                moveY = 0;
                
                if (sX < portal.Min()) return;
                double d = MyMath.Geometry.Tan(sX, portal.Max());
                if (d <= portal.Wide)
                    return;
            }
            else
            {
                sX = p.X;
                sY = p.Y + moveY;
                moveX = 0;
                if (sY < portal.Min()) return;

                double d = MyMath.Geometry.Tan(sY, portal.Max());
                if (d <= portal.Wide)
                    return;
            }

            pg.Figures[0].StartPoint = new Point(sX, sY);

            int count = pg.Figures[0].Segments.Count;
            for (int i = 0; i < count; ++i)
            {
                double _x = sX;
                double _y = sY;
                LineSegment ls = pg.Figures[0].Segments[i] as LineSegment;
                _x = ls.Point.X + moveX;                    
                _y = ls.Point.Y + moveY;
                ls.Point = new Point(_x, _y);
            }
        }

        private void MovePlace(double x, double y, double moveX, double moveY, Entity v)
        {
            Place place = v as Place;
            if (!place.IsMovable) return;

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
}
