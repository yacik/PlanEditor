using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;
using PlanEditor.Helpers;
using PlanEditor.Helpers.IO;
using PlanEditor.MyMath;
using PlanEditor.RegGrid;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Entities;

namespace PlanEditor
{
    public partial class MainWindow : Window
    {
        private enum CanvasMode
        {
            Move,
            Path,
            Select,
            Connect
        }

        private CanvasMode _mode;

        private TransformGroup _transformGroup;
        private TranslateTransform _translation;
        private ScaleTransform _scale;
        private double _lastPosX;
        private double _lastPosY;
        private Line _line;
        private Entities.Building _building;
        private bool _firstClick = false;
        private int _curStage = 0;
        private bool _isConnected = true;
        private Place _place1 = null;
        private Place _place2 = null;
        private RegGrid.Grid _grid;
        private readonly List<Entity> _selected = new List<Entity>();
        private readonly List<Line> _lines = new List<Line>();
        private readonly List<Line> _drawGrid = new List<Line>();
        private bool _shiftPressed;


        private Point _clickOne;
        private Point _clickTwo;

        public MainWindow()
        {
            InitializeComponent();

            _transformGroup = new TransformGroup();
            _translation = new TranslateTransform();
            _scale = new ScaleTransform();
            _transformGroup.Children.Add(_translation);
            _transformGroup.Children.Add(_scale);

            ContentPanel.IsEnabled = false;
            MenuAdd.IsEnabled = false;
            MenuTools.IsEnabled = false;

            _building = new Entities.Building();
            _grid = new RegGrid.Grid(_building);

            Stage.SetValue(Panel.ZIndexProperty, 100);
            _mode = CanvasMode.Move;

            #region MouseEvents

            ContentPanel.MouseMove += GridField_MouseMove;
            //ContentPanel.MouseRightButtonDown += GridField_MouseRightButtonDown;
            ContentPanel.MouseLeftButtonUp += GridField_MouseLeftButtonUp;
            ContentPanel.MouseLeftButtonDown += GridField_MouseLeftButtonDown;
            ContentPanel.MouseDown += GridField_MouseDown;
            ContentPanel.MouseLeave += GridField_MouseLeave;
            ContentPanel.MouseWheel += GridField_MouseWheel;
            ContentPanel.KeyDown += GridField_KeyDown;
            ContentPanel.KeyUp += GridField_KeyUp;
            ContentPanel.Focusable = true;
            ContentPanel.Visibility = Visibility.Visible;
            ContentPanel.IsEnabled = true;
            ContentPanel.Focus();

            #endregion
        }

        private void GridField_KeyDown(object sender, KeyEventArgs e)
        {
            switch (_mode)
            {
                case CanvasMode.Path:
                    if (Key.Escape == e.Key)
                    {
                        _firstClick = true;
                        _line = null;
                        _lines.Clear();
                    }
                    break;
            }
            _shiftPressed = (Key.LeftShift == e.Key);
        }

        private void GridField_KeyUp(object sender, KeyEventArgs e)
        {
            _shiftPressed = (Key.LeftShift != e.Key);
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

            switch (_mode)
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
            double x = e.GetPosition(null).X - _translation.X;
            double y = e.GetPosition(null).Y - _translation.Y;

            x /= _scale.ScaleX;
            y /= _scale.ScaleY;

            if (e.ClickCount >= 2)
            {
                DeselectAll();
            }

            switch (_mode)
            {
                case CanvasMode.Path:
                    if (e.ClickCount >= 2)
                    {
                        MouseRightButtonDownPath();
                    }
                    else
                    {
                        if (!_firstClick)
                        {
                            _firstClick = true;

                            AddNewLine(x, y);
                        }
                        else
                        {
                            _line.X2 = x;
                            _line.Y2 = y;

                            AddNewLine(x, y);
                        }
                    }
                    break;

                case CanvasMode.Move:
                    _lastPosX = e.GetPosition(null).X;
                    _lastPosY = e.GetPosition(null).Y;
                    break;

                case CanvasMode.Select:                 
                    _lastPosX = e.GetPosition(null).X;
                    _lastPosY = e.GetPosition(null).Y;
                    SelectObjects(x, y);
                    break;
                case CanvasMode.Connect:
                    ConnectPlaces(x, y);
                    break;
            }
        }

        public void DrawGrid()
        {
            _grid = new RegGrid.Grid(_building);
            _grid.CreateGrid();

            foreach (var v in _drawGrid)
                ContentPanel.Children.Remove(v);

            _drawGrid.Clear();
            
            for (int m = 0; m < _building.Row + 1; ++m)
            {
                var l = new Line
                    {
                        X1 = m*Data.GridStep,
                        Y1 = 0,
                        X2 = m*Data.GridStep,
                        Y2 = _building.yMax,
                        Stroke = Colours.LightGray,
                        StrokeThickness = 1,
                        RenderTransform = _transformGroup,
                    };
                    
                Panel.SetZIndex(l, -1);

                ContentPanel.Children.Add(l);

                _drawGrid.Add(l);
            }

            for (int n = 0; n < _building.Col + 1; ++n)
            {
                var l = new Line
                    {
                        X1 = 0,
                        Y1 = n*Data.GridStep,
                        X2 = _building.xMax,
                        Y2 = n*Data.GridStep,
                        Stroke = Colours.LightGray,
                        StrokeThickness = 1,
                        RenderTransform = _transformGroup
                    };

                Panel.SetZIndex(l, -1);

                ContentPanel.Children.Add(l);

                _drawGrid.Add(l);
            }
        }

        private void Click_New(object sender, RoutedEventArgs e)
        {
            var bldg = new Building(_building, Building.Mode.New);
            var isOk = bldg.ShowDialog();
            if (isOk == null) return;

            if (isOk == true)
            {
                CreateNewProject();
            }
            ChangeStageName();
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {

            var dlg = new SaveFileDialog { DefaultExt = ".rintd", Filter = "RINTD Files (*.rintd) |*rintd" };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                SaveFile.Save(dlg.FileName, _building);
            }
        }
        
        private void Click_Export(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { DefaultExt = ".txt", Filter = "Text Files (*.txt) |*txt" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                var rg = new RecognizeGrid(_grid, _building);
                rg.Recognize();
                SaveToEvac.Save(dlg.FileName, rg.Grid, _building);
            }
        }

        private void Click_Open(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { DefaultExt = ".rintd", Filter = "RINTD Files (*.rintd) |*rintd" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                LoadBinary(dlg.FileName);
            }

            ContentPanel.Children.Clear();
            _curStage = 0;

            _transformGroup = new TransformGroup();
            _translation = new TranslateTransform();
            _scale = new ScaleTransform();
            _transformGroup.Children.Add(_translation);
            _transformGroup.Children.Add(_scale);

            CreateNewProject();

            for (int i = 0; i < _building.Stages; ++i)
            {
                if (_building.Places.Count > i)
                    foreach (var v in _building.Places[i])
                    {
                        ContentPanel.Children.Add(v.UI);
                        v.UI.RenderTransform = _transformGroup;
                        if (i != _curStage)
                            v.UI.Visibility = Visibility.Hidden;
                    }
                
                int s = _curStage + 1;
                foreach (var v in _building.Mines.Where(v => !ContentPanel.Children.Contains(v.UI)))
                {
                    ContentPanel.Children.Add(v.UI);
                    v.UI.RenderTransform = _transformGroup;
                    if (s <= v.StageFrom && s >= v.StageTo)
                        v.UI.Visibility = Visibility.Hidden;
                }

                if (_building.Portals.Count > i)
                    foreach (var v in _building.Portals[i])
                    {
                        ContentPanel.Children.Add(v.UI);
                        v.UI.RenderTransform = _transformGroup;
                        if (i != _curStage)
                            v.UI.Visibility = Visibility.Hidden;
                    }
            }

            DrawPlan();
            ChangeStageName();
        }

        private void LoadBinary(string fileName)
        {
            _building = new Entities.Building();
            var lf = new LoadFile(fileName, _building);
            lf.Load();
            _building = lf.Building;
        }

        private void CreateNewProject()        
        {
            _curStage = 0;
            ContentPanel.IsEnabled = true;
            MenuAdd.IsEnabled = true;
            MenuTools.IsEnabled = true;

            _building.xMax = _building.Lx / Data.Sigma;
            _building.yMax = _building.Ly / Data.Sigma;

            _building.Row = (int)(_building.xMax / Data.GridStep);
            _building.Col = (int)(_building.yMax / Data.GridStep);

            DrawGrid();

            if (!ContentPanel.Children.Contains(Stage))
                ContentPanel.Children.Add(Stage);
        }

        private void Click_Path(object sender, RoutedEventArgs e)
        {
            _mode = CanvasMode.Path;
            DeselectAll();
        }

        private void Click_SelectTool(object sender, RoutedEventArgs e)
        {
            _mode = CanvasMode.Select;
        }

        private void Click_MoveTool(object sender, RoutedEventArgs e)
        {
            _mode = CanvasMode.Move;
            DeselectAll();
        }
        
        private void Click_AddRoom(object sender, RoutedEventArgs e)
        {            
            var r = new Room {Owner = this};
            var isOk = r.ShowDialog();

            if (isOk == null || isOk == false)
            {
                // remove all data
            }
            else if (isOk == true)
            {
                var entity = r.Entity;         
                entity.UI.RenderTransform = _transformGroup;
                ContentPanel.Children.Add(entity.UI);

                switch (entity.Type)
                {
                    case Entity.EntityType.Stairway:
                        var stairway = entity as Stairway;
                        _building.Mines.Add(stairway);
                        break;
                    case Entity.EntityType.Place:
                    case Entity.EntityType.Halfway:
                        var place = entity as Place;
                        _building.Places[_curStage].Add(place);
                    break;
                }                
            }            
        }

        private void Click_AddDoor(object sender, RoutedEventArgs e)
        {
            _mode = CanvasMode.Connect;
        }
        
        private void Click_EditBuilding(object sender, RoutedEventArgs e)
        {
            double lx = _building.Lx;
            double ly = _building.Ly;

            var bld = new Building(_building, Building.Mode.Edit);
            var isOk = bld.ShowDialog();

            if (isOk != null && isOk == true)
            {
                if (lx != _building.Lx || ly != _building.Ly)
                {
                    _building.xMax = _building.Lx / Data.Sigma;
                    _building.yMax = _building.Ly / Data.Sigma;

                    _building.Row = (int)(_building.xMax / Data.GridStep);
                    _building.Col = (int)(_building.yMax / Data.GridStep);

                    DrawGrid();
                }
            }
        }

        private void Click_NextStage(object sender, RoutedEventArgs e)
        {
            if (_curStage == _building.Stages - 1) return;
            
            SetHidden();
            ++_curStage;
            SetVisible();
            ChangeStageName();
        }

        private void Click_PrevStage(object sender, RoutedEventArgs e)
        {
            if (_curStage == 0) return;

            SetHidden();
            --_curStage;
            SetVisible();
            ChangeStageName();
        }

        private void SetHidden()
        {
            if (_building.Places.Count > _curStage)
            {
                foreach (var v in _building.Places[_curStage])
                {
                    v.UI.Visibility = Visibility.Hidden;
                }
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var v in _building.Portals[_curStage])
                {
                    v.UI.Visibility = Visibility.Hidden;
                }
            }

            int s = _curStage + 1;
            foreach (var v in _building.Mines)
            {
                if (s >= v.StageFrom && s <= v.StageTo)
                    v.UI.Visibility = Visibility.Visible;
                else
                    v.UI.Visibility = Visibility.Hidden;
            }
        }

        private void SetVisible()
        {
            if (_building.Places.Count > _curStage)
            {
                foreach (var v in _building.Places[_curStage])
                {
                    v.UI.Visibility = Visibility.Visible;
                }
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var v in _building.Portals[_curStage])
                {
                    v.UI.Visibility = Visibility.Visible;
                }
            }
            
            int s = _curStage + 1;
            foreach (var v in _building.Mines)
            {
                if (s >= v.StageFrom && s <= v.StageTo)
                    v.UI.Visibility = Visibility.Visible;
                else
                    v.UI.Visibility = Visibility.Hidden;
            }
        }

        private void AddNewLine(double x, double y)
        {
            _line = new Line
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x,
                    Y2 = y,
                    Stroke = Colours.Black,
                    StrokeThickness = 1,
                    RenderTransform = _transformGroup
                };

            ContentPanel.Children.Add(_line);
            _lines.Add(_line);
        }

        private void MovePath(double x, double y)
        {
            if (!_firstClick && _line == null) return;

            x = x - _translation.X;
            y = y - _translation.Y;

            x /= _scale.ScaleX;
            y /= _scale.ScaleY;

            _line.X2 = x;
            _line.Y2 = y;
        }

        private void DrawPlan()
        {
            if (_building.Places.Count > _curStage)
                foreach (var p in _building.Places[_curStage])
                    p.UI.Visibility = Visibility.Visible;

            if (_building.Portals.Count > _curStage)
                foreach (var p in _building.Portals[_curStage])
                    p.UI.Visibility = Visibility.Visible;

            int s = _curStage + 1;
            foreach (var v in _building.Mines)
            {
                if (s >= v.StageFrom && s <= v.StageTo)
                    v.UI.Visibility = Visibility.Visible;
                else
                    v.UI.Visibility = Visibility.Hidden;
            }
        }

        private void Move(double x, double y)
        {
            double dX = Helper.Tan(x, _lastPosX);
            double dY = Helper.Tan(y, _lastPosY);

            dX /= _scale.ScaleX;
            dY /= _scale.ScaleY;

            if (x > _lastPosX) 
                _translation.X += dX;
            else
                _translation.X -= dX;

            if (y > _lastPosY)
                _translation.Y += dY;
            else
                _translation.Y -= dY;

            _lastPosX = x;
            _lastPosY = y;
        }

        private void MouseRightButtonDownPath()
        {
            if (_lines.Count == 0) return;

            _line.X2 = _lines[0].X1;
            _line.Y2 = _lines[0].Y1;

            var pg = new PathGeometry {FillRule = FillRule.Nonzero};

            var pf = new PathFigure();
            pg.Figures.Add(pf);

            pf.StartPoint = new Point(_lines[0].X1, _lines[0].Y1);

            for (int i = 1; i < _lines.Count; ++i)
            {
                var ls = new LineSegment {Point = new Point(_lines[i].X1, _lines[i].Y1)};
                pf.Segments.Add(ls);
            }

            var line = new LineSegment {Point = new Point(_lines[0].X1, _lines[0].Y1)};
            pf.Segments.Add(line);

            foreach (var l in _lines)
                ContentPanel.Children.Remove(l);

            var p = new Path
                {
                    Fill = Colours.Indigo,
                    StrokeThickness = 2,
                    Stroke = Colours.Black,
                    Data = pg,
                    RenderTransform = _transformGroup
                };

            var pl = new Place {UI = p};
            _building.Places[_curStage].Add(pl);
            
            ContentPanel.Children.Add(p);
                       
            _firstClick = false;
            _lines.Clear();
            _line = null;
        }
        
        private void MoveSelected(double x, double y)
        {
            double dX = Helper.Tan(x, _lastPosX);
            double dY = Helper.Tan(y, _lastPosY);

            dX /= _scale.ScaleX;
            dY /= _scale.ScaleY;

            double moveX = 0;
            double moveY = 0;

            if (x > _lastPosX)
                moveX += dX;
            else
                moveX -= dX;

            if (y > _lastPosY)
                moveY += dY;
            else
                moveY -= dY;

            _lastPosX = x;
            _lastPosY = y;

            foreach (var v in _selected)
            {
                switch(v.Type)
                {
                    case Entity.EntityType.Portal:
                        MovePortal(moveX, moveY, v);
                        break;
                    case Entity.EntityType.Place:
                    case Entity.EntityType.Stairway:
                    case Entity.EntityType.Halfway:
                    case Entity.EntityType.Lift:
                        MovePlace(moveX, moveY, v);
                        break;
                }
            }
        }

        private void Click_Property(object sender, RoutedEventArgs e)
        {
            /*Room room;
            switch (_selected.Count)
            {
                case 0:
                    return;
                case 1:
                    var place = _selected[0] as Place;
                    if (place==null) return;
                    room = new Room(place);
                    room.ShowDialog();
                    break;
                default:
                    var lst = _selected.Where(entity => entity.Type == Entity.EntityType.Place).OfType<Place>().ToList();
                    room = new Room(lst);
                    room.ShowDialog();
                    break;
            }*/
            if (_selected.Count != 1) return;
            switch (_selected[0].Type)
            {
                case Entity.EntityType.Portal:
                    var portal = _selected[0] as Portal;
                    if (portal == null) return;
                    var door = new Door(portal);
                    var isOK = door.ShowDialog();
                    //if (isOK != null && isOK == true)
                    //    portal.CreateUI(door.Wide);
                    break;
                default:
                    var place = _selected[0] as Place;
                    if (place==null) return;
                    var room = new Room(place);
                    room.ShowDialog();
                    break;
            }

        }

        private void SelectObjects(double x, double y)
        {
            if (_building.Places.Count > _curStage)
            {
                foreach (var p in _building.Places[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                {
                    if (_shiftPressed)
                    {
                        if (_selected.Contains(p))
                        {
                            _selected.Remove(p);
                            p.Deselect();
                        }
                        else
                        {
                            _selected.Add(p);
                            p.Select();
                        }
                    }
                    else
                    {
                        foreach (var entity in _selected) entity.Deselect();
                        p.Select();
                        _selected.Clear();
                        _selected.Add(p);
                    }
                }
            }

            foreach (var p in _building.Mines.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
            {
                if (_shiftPressed)
                {
                    if (_selected.Contains(p))
                    {
                        _selected.Remove(p);
                        p.Deselect();
                    }
                    else
                    {
                        _selected.Add(p);
                        p.Select();
                    }
                }
                else
                {
                    foreach (var entity in _selected) entity.Deselect();
                    p.Select();
                    _selected.Clear();
                    _selected.Add(p);
                }
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var p in _building.Portals[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                {
                    if (_shiftPressed)
                    {
                        if (_selected.Contains(p))
                        {
                            _selected.Remove(p);
                            p.Deselect();
                        }
                        else
                        {
                            _selected.Add(p);
                            p.Select();
                        }
                    }
                    else
                    {
                        foreach (var entity in _selected) entity.Deselect();
                        p.Select();
                        _selected.Clear();
                        _selected.Add(p);
                    }
                }
            }
        }
        
        private void DeselectAll()
        {
            foreach (var v in _selected)
                v.Deselect();

            _selected.Clear();
        }

        private void Click_Remove(object sender, RoutedEventArgs e)
        {
            foreach (var v in _selected)
            {
                switch (v.Type)
                {
                    case Entity.EntityType.Place:
                    case Entity.EntityType.Halfway:
                        var place = v as Place;
                        if (place == null) continue;
                        var toDelete = new List<Portal>();
                        foreach (var p in _building.Portals[_curStage])
                        {
                            bool isDoor = false;
                            if (p.RoomA == null && p.RoomB == place)
                                isDoor = true;
                            else if (p.RoomB == null && p.RoomA == place)
                                isDoor = true;
                            else if (p.RoomA == place || p.RoomB == place)
                            {
                                if (p.RoomA != place)
                                    p.RoomA.IsMovable = true;
                                else
                                    p.RoomB.IsMovable = true;

                                isDoor = true;
                            }

                            if (isDoor)
                            {
                                ContentPanel.Children.Remove(p.UI);
                                toDelete.Add(p);
                            }
                        }

                        ContentPanel.Children.Remove(place.UI);
                        _building.Places[_curStage].Remove(place);

                        foreach (var p in toDelete)
                            _building.Portals[_curStage].Remove(p);

                        toDelete.Clear();

                        break;
                    case Entity.EntityType.Portal:
                        var portal = v as Portal;
                        if (portal == null) continue;
                        ContentPanel.Children.Remove(portal.UI);
                        
                        if (portal.RoomA != null)
                            portal.RoomA.IsMovable = true;
                        if (portal.RoomB != null)
                            portal.RoomB.IsMovable = true;

                        _building.Portals[_curStage].Remove(portal);
                        break;
                    case Entity.EntityType.Stairway:
                        var stairway = v as Stairway;
                        if (stairway == null) continue;
                        ContentPanel.Children.Remove(stairway.UI);
                        _building.Mines.Remove(stairway);
                        break;
                }
            }

            _selected.Clear();
        }

        #region Connect portals

        private void ConnectOutsidePortal(double wide)
        {
            var place = _place1 ?? _place2;

            var portal = new Portal
                {
                    RoomA = place,
                    RoomB = null,
                    Wide = wide
                };
            
            portal.RoomA.IsMovable = false;

            var min = double.MaxValue;
            var lines = place.Lines;
            Line line = null;


            foreach (var v in lines)
            {
                var d = Helper.IsHorizontal(v.X1, v.Y1, v.X2, v.Y2) ? Helper.Tan(_lastPosY, v.Y1) : Helper.Tan(_lastPosX, v.X1);

                if (d > min) continue;

                min = d;
                line = v;
            }
            
            if (line == null) return;

            if (Helper.IsHorizontal(line.X1, line.Y1, line.X2, line.Y2))
            {
                portal.Orientation = Portal.PortalOrient.Horizontal;                
                portal.Min = Math.Min(line.X1, line.X2);
                portal.Max = Math.Max(line.X1, line.X2);
                portal.CreateUI(line.Y2);
            }
            else
            {
                portal.Orientation = Portal.PortalOrient.Vertical;
                portal.Min = Math.Min(line.Y1, line.Y2);
                portal.Max = Math.Max(line.Y1, line.Y2);
                portal.CreateUI(line.X1);
            }

            portal.UI.RenderTransform = _transformGroup;
            ContentPanel.Children.Add(portal.UI);
            _building.Portals[_curStage].Add(portal);

            _place1 = null;
            _place2 = null;
        }
        
        private void ConnectPlaces(double x, double y)
        {
            if (_isConnected)
            {
                _clickOne = new Point(x, y);

                _isConnected = !_isConnected;

                foreach (var p in _building.Places[_curStage])
                {
                    if (Helper.IsCollide(x, y, p.PointsX, p.PointsY))
                    {
                        _place1 = p;
                        break;
                    }
                }

                foreach (var p in _building.Mines)
                {
                    if (Helper.IsCollide(x, y, p.PointsX, p.PointsY))
                    {
                        _place1 = p;
                        break;
                    }
                }
            }
            else if (!_isConnected)
            {
                _clickTwo = new Point(x, y);

                _isConnected = !_isConnected;

                foreach (
                    var p in
                        _building.Places[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY))
                                                   .Where(p => _place1 != p))
                {
                    _place2 = p;
                    break;
                }

                foreach (
                    var p in
                        _building.Mines.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY))
                                 .Where(p => _place1 != p))
                {
                    _place2 = p;
                    break;
                }

                _lastPosX = x;
                _lastPosY = y;

                if (_place1 == null && _place2 == null)
                {
                    MessageBox.Show("Помещения не выбраны");
                    return;
                }

                var door = new Door {Owner = this};
                var isOk = door.ShowDialog();

                if (isOk != null && isOk == true)
                {
                    var wide = door.Wide;
                    if (wide == -1) MessageBox.Show("Ширина задана не верно");
                    else
                    {
                        if (_place1 == null || _place2 == null)
                            ConnectOutsidePortal(wide);
                        if (_place1 != null && _place2 != null)
                            ConncetInnerPortal(wide);
                    }
                }
            }
        }

        private void ConncetInnerPortal(double wide)
        {
            double compWide = wide/Data.Sigma;
            double dX = Helper.Tan(_clickOne.X, _clickTwo.X);
            double dY = Helper.Tan(_clickOne.Y, _clickTwo.Y);

            if (dX > dY)
            {
                if (_clickOne.X > _clickTwo.X)
                {
                    var tmp = _place2;
                    _place2 = _place1;
                    _place1 = tmp;
                }
            }
            else
            {
                if (_clickOne.Y > _clickTwo.Y)
                {
                    var tmp = _place2;
                    _place2 = _place1;
                    _place1 = tmp;
                }
            }

            var lines1 = _place1.Lines;
            var lines2 = _place2.Lines;

            _selected.Clear();

            _place1.Select();
            _place2.Select();
            _selected.Add(_place1);
            _selected.Add(_place2);

            var isVerNeigh = DefineNeighRooms(_place1, _place2);

            var max = 0.00;
            var startPoint = 0.00;
            var min = 0.00;
            var orient = Portal.PortalOrient.Vertical;
            var isFound = false;

            foreach (var line1 in lines1)
            {
                var isHor1 = Helper.IsHorizontal(line1.X1, line1.Y1, line1.X2, line1.Y2);

                foreach (var line2 in lines2)
                {
                    var isHor2 = Helper.IsHorizontal(line2.X1, line2.Y1, line2.X2, line2.Y2);

                    if (isHor1 && isHor2 && isVerNeigh)
                    {
                        if (!IsNearest(line1.Y1, line1.Y2, line2.Y1, line2.Y2)) continue;

                        var mas1 = GetMinMax(line1.X1, line1.X2);
                        var mas2 = GetMinMax(line2.X1, line2.X2);

                        min = Math.Max(mas1[0], mas2[0]);
                        max = Math.Min(mas1[1], mas2[1]);

                        var d = max - min;

                        if (d < compWide)
                        {
                            MessageBox.Show("Ширина двери привышает размеры стен");
                            continue;
                        }

                        double dist = Helper.Tan(line1.Y1, line2.Y1)/2;
                        startPoint = line1.Y1 + dist;

                        orient = Portal.PortalOrient.Horizontal;

                        isFound = true;
                    }
                    else if (!isHor1 && !isHor2 && !isVerNeigh)
                    {
                        if (!IsNearest(line1.X1, line1.X2, line2.X1, line2.X2)) continue;

                        var mas1 = GetMinMax(line1.Y1, line1.Y2);
                        var mas2 = GetMinMax(line2.Y1, line2.Y2);

                        min = Math.Max(mas1[0], mas2[0]);
                        max = Math.Min(mas1[1], mas2[1]);

                        var d = max - min;

                        if (d < compWide)
                        {
                            MessageBox.Show("Ширина двери привышает размеры стен");
                            continue;
                        }

                        double dist = Helper.Tan(line1.X1, line2.X1)/2;
                        startPoint = line1.X1 + dist;

                        orient = Portal.PortalOrient.Vertical;

                        isFound = true;
                    }
                }
            }

            if (isFound)
                CreatePortal(min, max, _place1, _place2, orient, startPoint, wide);

            _place1 = null;
            _place2 = null;
        }

        public bool IsNearest(double a1, double a2, double b1, double b2)
        {
            var v1 = Helper.Tan(a1, b1);
            var v2 = Helper.Tan(a1, b2);
            var v3 = Helper.Tan(a2, b1);
            var v4 = Helper.Tan(a2, b2);

            var mas = new[] {v1, v2, v3, v4};
            var min = mas.Concat(new[] {double.MaxValue}).Min();

            return min < Data.GridStep;
        }

        private bool DefineNeighRooms(Place place1, Place place2)
        {
            var mas1 = new[] {double.MaxValue, double.MinValue, double.MaxValue, double.MinValue};
            var mas2 = new[] {double.MaxValue, double.MinValue, double.MaxValue, double.MinValue};

            foreach (var line in place1.Lines)
            {
                var x1 = Math.Min(line.X1, line.X2);
                var x2 = Math.Max(line.X1, line.X2);
                var y1 = Math.Min(line.Y1, line.Y2);
                var y2 = Math.Max(line.Y1, line.Y2);

                if (mas1[0] > x1) mas1[0] = x1;
                if (mas1[1] < x2) mas1[1] = x2;
                if (mas1[2] > y1) mas1[2] = y1;
                if (mas1[3] < y2) mas1[3] = y2;
            }

            foreach (var line in place2.Lines)
            {
                var x1 = Math.Min(line.X1, line.X2);
                var x2 = Math.Max(line.X1, line.X2);
                var y1 = Math.Min(line.Y1, line.Y2);
                var y2 = Math.Max(line.Y1, line.Y2);

                if (mas2[0] > x1) mas2[0] = x1;
                if (mas2[1] < x2) mas2[1] = x2;
                if (mas2[2] > y1) mas2[2] = y1;
                if (mas2[3] < y2) mas2[3] = y2;
            }

            var ay = mas1[3] - mas2[2];
            var by = mas2[2] - mas1[3];

            var tmpY = Math.Min(ay, by);
            var y = Math.Sqrt(tmpY*tmpY);

            var ax = mas1[1] - mas2[0];
            var bx = mas2[0] - mas1[1];

            var tmpX = Math.Min(ax, bx);
            var x = Math.Sqrt(tmpX*tmpX);

            return x > y;
        }

        private void CreatePortal(double min, double max, Place place1, Place place2, Portal.PortalOrient orientation,
                                  double pos, double wide)
        {
            var portal = new Portal
                {
                    RoomA = place1,
                    RoomB = place2,
                    Min = min,
                    Max = max,
                    Orientation = orientation,
                    Wide = wide,
                };

            portal.RoomA.IsMovable = false;
            portal.RoomB.IsMovable = false;

            _building.Portals[_curStage].Add(portal);

            portal.CreateUI(pos);
            portal.UI.RenderTransform = _transformGroup;
            ContentPanel.Children.Add(portal.UI);
        }

        private double[] GetMinMax(double x1, double x2)
        {
            return x1 < x2 ? new [] { x1, x2 } : new [] { x2, x1 };
        }

        #endregion
        
        private void MovePortal(double moveX, double moveY, Entity v)
        {
            var portal = v as Portal;
            if (portal == null) return;

            var pg = v.UI.Data as PathGeometry;
            var p = pg.Figures[0].StartPoint;
            double x = p.X;
            double y = p.Y;

            if (portal.Orientation == Portal.PortalOrient.Horizontal)
            {                
                x = p.X + moveX;
                y = p.Y;
                moveY = 0;
                
                if (x < portal.Min) return;
                double d = Helper.Tan(x, portal.Max);
                double wide = portal.Wide/Data.Sigma;
                if (d <= wide)
                    return;
            }
            else
            {
                x = p.X;
                y = p.Y + moveY;
                moveX = 0;
                if (y < portal.Min) return;

                double d = Helper.Tan(y, portal.Max);
                double wide = portal.Wide / Data.Sigma;
                if (d <= wide)
                    return;
            }

            pg.Figures[0].StartPoint = new Point(x, y);

            int count = pg.Figures[0].Segments.Count;
            for (int i = 0; i < count; ++i)
            {
                double _x = x;
                double _y = y;
                var ls = pg.Figures[0].Segments[i] as LineSegment;
                _x = ls.Point.X + moveX;                    
                _y = ls.Point.Y + moveY;
                ls.Point = new Point(_x, _y);
            }
        }

        private void MovePlace(double moveX, double moveY, Entity v)
        {
            var place = v as Place;
            if (place == null || !place.IsMovable) return;

            var pg = v.UI.Data as PathGeometry;
            var p = pg.Figures[0].StartPoint;
            double x = p.X + moveX;
            double y = p.Y + moveY;
            pg.Figures[0].StartPoint = new Point(x, y);

            int count = pg.Figures[0].Segments.Count;
            for (int i = 0; i < count; ++i)
            {
                var ls = pg.Figures[0].Segments[i] as LineSegment;
                if (ls != null)
                {
                    x = ls.Point.X + moveX;
                    y = ls.Point.Y + moveY;
                    ls.Point = new Point(x, y);
                }
            }                
        }
        private void ChangeStageName()
        {
            int s = _curStage + 1;                            
            Stage.Text = "Этаж " + s;
        }
    }    
}
