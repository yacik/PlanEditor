using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;
using PlanEditor.Helpers;
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
            Connect,
            Edit
        }

        private CanvasMode _mode;

        private TransformGroup _transformGroup;
        private TranslateTransform _translation;
        private ScaleTransform _scale;
        private double _lastPosX;
        private double _lastPosY;
        private Line _line;
        private Building _building;
        private bool _firstClick;
        private int _curStage;
        private bool _isConnected = true;
        private Place _place1;
        private Place _place2;
        private Point _clickOne;
        private Point _clickTwo;
        private Point _lastClick = new Point(100, 100);
        private Point _curPosMouse;
        private RegGrid.Grid _grid;
        private readonly List<Entity> _selectedItems = new List<Entity>();
        private readonly List<Obstacle> _obstacles = new List<Obstacle>();
        private readonly List<Line> _lines = new List<Line>();
        private readonly List<Line> _drawGrid = new List<Line>();
        private bool _shiftPressed;
        private bool _isChanged;
        private Entity _selectedItem;

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

            _building = new Building();
            _grid = new RegGrid.Grid(_building);

            Stage.SetValue(Panel.ZIndexProperty, 100);
            _mode = CanvasMode.Select;
            
            #region MouseEvents

            ContentPanel.MouseMove += GridField_MouseMove;
            ContentPanel.MouseLeftButtonUp += GridField_MouseLeftButtonUp;
            ContentPanel.MouseLeftButtonDown += GridField_MouseLeftButtonDown;
            ContentPanel.MouseRightButtonDown += GridField_MouseRightButtonDown;
            ContentPanel.MouseDown += GridField_MouseDown;
            ContentPanel.MouseLeave += GridField_MouseLeave;
            ContentPanel.MouseDown += GridField_MouseDown;
            ContentPanel.KeyDown += GridField_KeyDown;
            ContentPanel.KeyUp += GridField_KeyUp;
            ContentPanel.Focusable = true;
            ContentPanel.Visibility = Visibility.Visible;
            ContentPanel.IsEnabled = true;
            ContentPanel.Focus();

            #endregion
        }

        #region GridFields

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
                case CanvasMode.Edit:
                    if (Key.Escape == e.Key)
                    {
                        SetVisible();
                        SetVisibleStairways();

                        foreach (var obstacle in _obstacles) obstacle.Deselect();
                        _obstacles.Clear();
                        
                        if (_selectedItem != null)
                            _selectedItem.Deselect();
                        
                        _mode = CanvasMode.Select;
                        ActiveDeactiveMenu(true);
                        AddObstacle.IsEnabled = false;
                    }
                    break;
            }

            if (Key.LeftShift == e.Key) _shiftPressed = true;

            if (Key.C == e.Key)
            {
                foreach (var ptls in _building.Portals)
                {
                    foreach (var p in ptls)
                    {
                        if (p.RoomA == null || p.RoomB == null)
                        {
                            p.Select();
                        }
                    }
                }
            }
        }

        private void GridField_KeyUp(object sender, KeyEventArgs e)
        {
            if (Key.LeftShift == e.Key) _shiftPressed = false;
        }
        
        private void GridField_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void GridField_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _lastPosX = e.GetPosition(null).X;
                _lastPosY = e.GetPosition(null).Y;
            }
        }
        
        private void GridField_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(null).X;
            double y = e.GetPosition(null).Y;
            
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Move(x, y);
            }
            else
            {
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

                    case CanvasMode.Edit:
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            MoveSelected(x, y);
                        }
                        break;
                }
            }
        }
        
        private void GridField_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _lastPosX = e.GetPosition(null).X;
            _lastPosY = e.GetPosition(null).Y;
        }

        private void GridField_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(null).X - _translation.X;
            double y = e.GetPosition(null).Y - _translation.Y;
            
            _lastClick = (x < Data.GridStep || x > _building.xMax || y < Data.GridStep || y > _building.yMax)
                             ? _lastClick
                             : new Point(x, y);
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
                    if (e.ClickCount >= 2) DeselectAll();
                    _lastPosX = e.GetPosition(null).X;
                    _lastPosY = e.GetPosition(null).Y;
                    break;

                case CanvasMode.Select:
                    if (e.ClickCount >= 2)
                    {
                        _selectedItem = SelectObject(x, y);
                        if (_selectedItem == null)
                        {
                            DeselectAll();
                            return;
                        }

                        if (_selectedItem.Type == Entity.EntityType.Lift ||
                            _selectedItem.Type == Entity.EntityType.Portal) return;

                        var pl = _selectedItem as Place;
                        if (pl == null) return;

                        ActiveDeactiveMenu(false);
                        _mode = CanvasMode.Edit;

                        AddObstacle.IsEnabled = true;

                        SetHidden();
                        SetHiddenStairways();
                        pl.Show();
                        pl.ShowObstacles();
                        _obstacles.Clear();
                    }
                    else
                    {
                        _lastPosX = e.GetPosition(null).X;
                        _lastPosY = e.GetPosition(null).Y;

                        if (_shiftPressed) SelectObjects(x, y);
                        else
                        {
                            DeselectAll();
                            _selectedItem = SelectObject(x, y);
                            if (_selectedItem != null)  _selectedItem.Select();
                        }
                    }
                    break;

                case CanvasMode.Connect:
                    ConnectPlaces(x, y);
                    break;

                case CanvasMode.Edit:
                    var place = _selectedItem as Place;
                    if (place == null) return;

                    foreach (var obstacle in place.Obstacles.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                    {
                        if (_obstacles.Contains(obstacle)) continue;

                        obstacle.Select();
                        _obstacles.Add(obstacle);
                    }
                    break;
            }
        }

        private void GridField_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _curPosMouse = new Point(e.GetPosition(null).X, e.GetPosition(null).Y);           
        }
        
        #endregion
        
        #region Click events
        
        private void Click_AddObstacle(object sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;

            var owner = _selectedItem as Place;
            if (owner == null) return;

            var winObst = new WinObstacle(owner) {Owner = this};
            var result = winObst.ShowDialog();
            if (result == true)
            {
                int pos = owner.Obstacles.Count - 1;
                var obstacle = owner.Obstacles[pos];
                obstacle.UI.RenderTransform = _transformGroup;
                ContentPanel.Children.Add(obstacle.UI);
            }
        }
        
        private void Click_New(object sender, RoutedEventArgs e)
        {
            var bldg = new WinBuilding(_building, WinBuilding.Mode.New);
            var isOk = bldg.ShowDialog();
            if (isOk == null) return;

            if (isOk == true) CreateNewProject();
            ChangeStageName();

            _isChanged = true;
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }

        private void Click_EditTool(object sender, RoutedEventArgs e)
        {
            _mode = CanvasMode.Edit;
            ActiveDeactiveMenu(false);
        }

        private void Click_Export(object sender, RoutedEventArgs e)
        {
            if (_building == null) return;

            var dlg = new SaveFileDialog { DefaultExt = ".txt", Filter = "Text Files (*.txt) |*txt" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                var rg = new RecognizeGrid(_grid, _building);
                rg.Recognize();
                Helpers.IO.SaveToEvac.Save(dlg.FileName, rg.Grid, _building);
            }
        }

        private void Click_Open(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { DefaultExt = ".rintd", Filter = "RINTD Files (*.rintd) |*rintd" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                if (LoadBinary(dlg.FileName))
                {
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
                                
                                if (i != _curStage) v.UI.Visibility = Visibility.Hidden;
                                
                                foreach (var obstacle in v.Obstacles)
                                {
                                    ContentPanel.Children.Add(obstacle.UI);
                                    obstacle.UI.RenderTransform = _transformGroup;
                                    if (i != _curStage) obstacle.UI.Visibility = Visibility.Hidden;
                                }
                            }

                        int s = _curStage + 1;
                        foreach (var v in _building.Stairways.Where(v => !ContentPanel.Children.Contains(v.UI)))
                        {
                            ContentPanel.Children.Add(v.UI);
                            v.UI.RenderTransform = _transformGroup;
                            if (s <= v.StageFrom && s >= v.StageTo) v.UI.Visibility = Visibility.Hidden;
                        }

                        if (_building.Portals.Count > i)
                            foreach (var v in _building.Portals[i])
                            {
                                ContentPanel.Children.Add(v.UI);
                                v.UI.RenderTransform = _transformGroup;
                                if (i != _curStage) v.UI.Visibility = Visibility.Hidden;
                            }
                    }
                    
                    for (int i = 0; i < _building.Stages; ++i)
                    {
                        if (_building.Places[i] == null) continue;
                        var places = _building.Places[i];
                        foreach (var place in _building.Places[i])
                        {
                            CollidePlacesAfterLoad(place, places);
                        }
                    }

                    DrawPlan();
                    ChangeStageName();
                }
                else MessageBox.Show("Ошибка в чтении файла");
            }

            _isChanged = false;
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

        private void Click_Menu(object sender, RoutedEventArgs e)
        {
            if (_selectedItems.Count == 0 && _selectedItem == null)
            {
                MouseMenuEdit.IsEnabled = false;
                MouseMenuCheck.IsEnabled = false;
                MouseMenuRemove.IsEnabled = false;
            }
            else
            {
                MouseMenuEdit.IsEnabled = true;
                MouseMenuCheck.IsEnabled = (_selectedItems.Count == 0);
                MouseMenuRemove.IsEnabled = true;
            }
        }

        private void Click_AddRoom(object sender, RoutedEventArgs e)
        {            
            var r = new WinRoom(_lastClick) {Owner = this};
            var isOk = r.ShowDialog();

            if (isOk == null || isOk == false)
            {
                // remove all data
            }
            else if (isOk == true)
            {
                _isChanged = true;

                var entity = r.Entity;         
                entity.UI.RenderTransform = _transformGroup;
                ContentPanel.Children.Add(entity.UI);

                switch (entity.Type)
                {
                    case Entity.EntityType.Stairway:
                        var stairway = entity as Stairway;
                        _building.Stairways.Add(stairway);
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
            if (_mode != CanvasMode.Edit)
                _mode = CanvasMode.Connect;
        }
        
        private void Click_EditBuilding(object sender, RoutedEventArgs e)
        {
            var lx = _building.Lx;
            var ly = _building.Ly;

            var bld = new WinBuilding(_building, WinBuilding.Mode.Edit);
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
        
        private void Click_Remove(object sender, RoutedEventArgs e)
        {
            if (_selectedItems.Count > 0)
            {
                var mb = MessageBox.Show("Удалить выбранные элементы?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (mb == MessageBoxResult.No) return;
                
                _selectedItems.Add(_selectedItem);
                _selectedItem = null;

                 foreach (var v in _selectedItems) RemoveEntity(v);
                _selectedItems.Clear();
            }
            if (_selectedItem != null)
            {
                var mb = MessageBox.Show("Удалить выбранные элементы?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (mb == MessageBoxResult.No) return;

                if (_mode == CanvasMode.Edit)
                {
                    var place = _selectedItem as Place;
                    if (place == null) return;

                    double x = _curPosMouse.X - _translation.X;
                    double y = _curPosMouse.Y - _translation.Y;

                    Obstacle obst = null;
                    foreach (var obstacle in place.Obstacles.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                    {
                        obst = obstacle;
                    }

                    if (obst != null)
                    {
                        place.Obstacles.Remove(obst);
                        ContentPanel.Children.Remove(obst.UI);
                    }
                } 
                else
                {
                    RemoveEntity(_selectedItem);
                    _selectedItem = null;
                }
            }
        }

        private void RemoveEntity(Entity v)
        {
            switch (v.Type)
            {
                case Entity.EntityType.Portal:
                    RemovePortal(v);
                    break;
                case Entity.EntityType.Stairway:
                    var stairway = v as Stairway;
                    if (stairway == null) return;

                    ContentPanel.Children.Remove(stairway.UI);
                    _building.Stairways.Remove(stairway);

                    foreach (var obs in stairway.Obstacles)
                        ContentPanel.Children.Remove(obs.UI);
                    
                     stairway.Obstacles.Clear();

                    break;
                default:
                    var place = v as Place;
                    if (place == null) return;
                    var toDelete = new List<Portal>();
                    foreach (var p in _building.Portals[_curStage])
                    {
                        bool isDoor = false;

                        if (p.RoomA == null && p.RoomB == place) isDoor = true;
                        else if (p.RoomB == null && p.RoomA == place) isDoor = true;
                        else if (p.RoomA == place || p.RoomB == place)
                        {
                            if (p.RoomA != place) p.RoomA.IsMovable = true;
                            else p.RoomB.IsMovable = true;

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

                     foreach (var obs in place.Obstacles)
                        ContentPanel.Children.Remove(obs.UI);
                    
                     place.Obstacles.Clear();

                    break;
            }
        }

        private void SaveProject()
        {
            var dlg = new SaveFileDialog { DefaultExt = ".rintd", Filter = "RINTD Files (*.rintd) |*rintd" };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                Helpers.IO.SaveFile.Save(dlg.FileName, _building);
            }
        }

        private void RemovePortal(Entity v)
        {
            var portal = v as Portal;
            if (portal == null) return;

            ContentPanel.Children.Remove(portal.UI);
            _building.Portals[_curStage].Remove(portal);

            int delete1 = 1;
            int delete2 = 1;

            for (int i = 0; i < _building.Portals.Count; ++i)
            {
                if (_building.Portals[i] == null) continue;

                foreach (var ptl in _building.Portals[i])
                {
                    if (portal.RoomA != null)
                    {
                        if (ptl.RoomA == portal.RoomA) ++delete1;
                        else if (ptl.RoomB == portal.RoomA) ++delete1;
                    }
                    if (portal.RoomB != null)
                    {
                        if (ptl.RoomA == portal.RoomB) ++delete2;
                        else if (ptl.RoomB == portal.RoomB) ++delete2;
                    }
                }
            }

            if (portal.RoomA != null) portal.RoomA.IsMovable = (delete1 == 1);
            if (portal.RoomB != null) portal.RoomB.IsMovable = (delete2 == 1);
        }

        private void Click_Property(object sender, RoutedEventArgs e)
        {
            if (_mode == CanvasMode.Edit)
            {
                    
            }
            else
            {
                if (_selectedItem != null)
                {
                    switch (_selectedItem.Type)
                    {
                        case Entity.EntityType.Portal:
                            var portal = _selectedItem as Portal;
                            if (portal != null)
                            {
                                var door = new WinPortal(portal);
                                door.ShowDialog();
                            }
                            break;
                        default:
                            var place = _selectedItem as Place;
                            if (place != null)
                            {
                                var room = new WinRoom(place);
                                room.ShowDialog();
                            }
                            break;
                    }
                }
                else if (_selectedItems.Count > 0)
                {
                    // TODO: multiply select tool
                }
            }
        }
        
        #endregion
        
        #region Connect portals

        private void ConnectOutsidePortal(double wide)
        {
            var place = _place1 ?? _place2;
            var portal = new Portal
                {
                    RoomA = place,
                    RoomB = null,
                    Width = wide
                };
            
            portal.RoomA.IsMovable = false;
            place.Select();

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
            
            if (line == null)
            {
                MessageBox.Show("Невозможно соединить помещение");
                return;
            }

            if (Helper.IsHorizontal(line.X1, line.Y1, line.X2, line.Y2))
            {
                portal.Orientation = Portal.PortalOrient.Horizontal;                
                portal.Min = Math.Min(line.X1, line.X2);
                portal.Max = Math.Max(line.X1, line.X2);

                if (portal.Max < wide)
                {
                    MessageBox.Show("Ширина двери задана не верна");
                    return;
                }
                
                portal.CreateUI(line.Y2);
            }
            else
            {
                portal.Orientation = Portal.PortalOrient.Vertical;
                portal.Min = Math.Min(line.Y1, line.Y2);
                portal.Max = Math.Max(line.Y1, line.Y2);

                if (portal.Max < wide)
                {
                    MessageBox.Show("Ширина двери задана не верна");
                    return;
                }
                
                portal.CreateUI(line.X1);
            }

            var mb = MessageBox.Show("Создать дверь, ведущую на улицу?", "Подтверждение", MessageBoxButton.YesNo);
            if (mb == MessageBoxResult.Yes)
            {
                portal.UI.RenderTransform = _transformGroup;
                ContentPanel.Children.Add(portal.UI);
                _building.Portals[_curStage].Add(portal);
            }
            place.Deselect();

            _place1 = null;
            _place2 = null;
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

            _selectedItems.Clear();

            _place1.Select();
            _place2.Select();
            _selectedItems.Add(_place1);
            _selectedItems.Add(_place2);

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
                            MessageBox.Show("Ширина двери превышает размеры стен");
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
                            MessageBox.Show("Ширина двери превышает размеры стен");
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
            {
                Portal door = null;
                Place place = null;
                if (_place1.Type == Entity.EntityType.Stairway) place = _place1;
                if (_place2.Type == Entity.EntityType.Stairway) place = _place2;
                if (place != null)
                {
                    for (int i = 0; i < _building.Stages; ++i)
                    {
                        if (_building.Portals[i] == null) continue;
                        
                        foreach (var portal in _building.Portals[i])
                        {
                            if (portal.RoomA != null)
                            {
                                if (portal.RoomA == place || portal.RoomA == place)
                                {
                                    door = portal;
                                }
                            } 
                            if (portal.RoomB != null)
                            {
                                if (portal.RoomB == place || portal.RoomB == place)
                                {
                                    door = portal;
                                }
                            }
                        }
                    }
                }
                if (door != null)
                {
                    if (door.Width != wide)
                    {
                        MessageBox.Show("Размер двери не сооответствует ранее установленной");
                        wide = door.Width;
                    }
                }
                var mb = MessageBox.Show("Соединить данные помещения?", "Подтверждение", MessageBoxButton.YesNo);
                if (mb == MessageBoxResult.Yes)
                    CreatePortal(min, max, _place1, _place2, orient, startPoint, wide);
            }

            _place1.Deselect();
            _place2.Deselect();

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

                foreach (var p in _building.Stairways)
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
                        _building.Stairways.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY))
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

                if ((_place1 != null && _place1.IsCollide) || (_place2 != null && _place2.IsCollide))
                {
                    MessageBox.Show("Помещения пересекаются, невозможно соединить");
                    return;
                }

                var door = new WinPortal { Owner = this };
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
            _isChanged = true;

            if (max < wide)
            {
                MessageBox.Show("Ширина двери задана не верна");
                return;
            }

            var portal = new Portal
                {
                    RoomA = place1,
                    RoomB = place2,
                    Min = min,
                    Max = max,
                    Orientation = orientation,
                    Width = wide,
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

        #region Move

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
        
        private void MovePortal(double moveX, double moveY, Entity v)
        {
            _isChanged = true;

            var portal = (Portal)v;

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
                double wide = portal.Width/Data.Sigma;
                if (d <= wide) return;
            }
            else
            {
                x = p.X;
                y = p.Y + moveY;
                moveX = 0;
                if (y < portal.Min) return;

                double d = Helper.Tan(y, portal.Max);
                double wide = portal.Width / Data.Sigma;
                if (d <= wide) return;
            }

            pg.Figures[0].StartPoint = new Point(x, y);

            int count = pg.Figures[0].Segments.Count;
            for (int i = 0; i < count; ++i)
            {
                double _x = x;
                double _y = y;
                var ls = pg.Figures[0].Segments[i] as LineSegment;
                if (ls == null) continue;

                _x = ls.Point.X + moveX;                    
                _y = ls.Point.Y + moveY;
                ls.Point = new Point(_x, _y);
            }
        }

        private void MovePlace(double moveX, double moveY, Entity v)
        {
            _isChanged = true;

            var place = v as Place;
            if (place == null || !place.IsMovable) return;

            var pg = v.UI.Data as PathGeometry;
            var p = pg.Figures[0].StartPoint;
            double x = p.X + moveX;
            double y = p.Y + moveY;

            // Check for leaving canvas zone
            double wide = x + place.Wide / Data.Sigma;
            double height = y + place.Length / Data.Sigma;
            if (x < Data.GridStep || wide > _building.xMax - Data.GridStep) return;
            if (y < Data.GridStep || height > _building.yMax - Data.GridStep) return;

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

            if (place.Obstacles != null)
                foreach (var obstacle in place.Obstacles) MoveObstacle(moveX, moveY, obstacle);
        
            CollidePlaces(place, _building.Places[_curStage]);
        }

        private void MoveObstacle(double moveX, double moveY, Obstacle o)
        {
            var pg = o.UI.Data as PathGeometry;
            if (pg == null) return;

            var p = pg.Figures[0].StartPoint;
            double x = p.X + moveX;
            double y = p.Y + moveY;

            // Check for leaving canvas zone
            double wide = x + o.Wide / Data.Sigma;
            double height = y + o.Length / Data.Sigma;
            if (x < o.MinPoint.X || wide > o.MaxPoint.X) return;
            if (y < o.MinPoint.Y || height > o.MaxPoint.Y) return;

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

        private void CollidePlacesAfterLoad(Place place, List<Place> places)
        {
            var px1 = place.PointsX;
            var py1 = place.PointsY;

            foreach (var pl in places)
            {
                if (Equals(pl, place)) continue;

                bool isCollide = false;

                var px2 = pl.PointsX;
                var py2 = pl.PointsY;

                for (int i = 0; i < px1.Count; ++i)
                {
                    double x = px1[i];
                    double y = py1[i];

                    if (Helper.IsCollide(x, y, px2, py2)) isCollide = true;

                    x = px2[i];
                    y = py2[i];

                    if (Helper.IsCollide(x, y, px1, py1)) isCollide = true;
                }

                if (isCollide)
                {
                    place.Collide();
                    pl.Collide();
                    if (!pl.Collisions.Contains(place))
                        pl.Collisions.Add(place);
                }
            }

            foreach (var stairway in _building.Stairways)
            {
                if (Equals(stairway, place)) continue;

                bool isCollide = false;

                var px2 = stairway.PointsX;
                var py2 = stairway.PointsY;

                for (int i = 0; i < px1.Count; ++i)
                {
                    double x = px1[i];
                    double y = py1[i];

                    if (Helper.IsCollide(x, y, px2, py2)) isCollide = true;

                    x = px2[i];
                    y = py2[i];

                    if (Helper.IsCollide(x, y, px1, py1)) isCollide = true;
                }

                if (isCollide)
                {
                    place.Collide();
                    stairway.Collide();
                    if (!stairway.Collisions.Contains(place))
                        stairway.Collisions.Add(place);
                }
            }
        }

        private void CollidePlaces(Place place, List<Place> places)
        {
            var px1 = place.PointsX;
            var py1 = place.PointsY;

            bool isInterscept = false;
            foreach (var pl in places)
            {
                if (Equals(pl, place)) continue;

                bool isCollide = false;

                var px2 = pl.PointsX;
                var py2 = pl.PointsY;
                
                for (int i = 0; i < px1.Count; ++i)
                {
                    double x = px1[i];
                    double y = py1[i];

                    if (Helper.IsCollide(x, y, px2, py2)) isCollide = true;

                    x = px2[i];
                    y = py2[i];

                    if (Helper.IsCollide(x, y, px1, py1)) isCollide = true;
                }

                if (isCollide)
                {
                    place.Collide();
                    if (!place.Collisions.Contains(pl))
                        place.Collisions.Add(pl); 

                    pl.Collide();
                    if (!pl.Collisions.Contains(place))
                        pl.Collisions.Add(place);

                    isInterscept = true;
                }
                else
                {
                    if (place.Collisions.Contains(pl))
                        place.Collisions.Remove(pl);

                    if (pl.Collisions.Contains(place))
                        pl.Collisions.Remove(place);

                    if (pl.Collisions.Count == 0)
                        pl.NonCollide();
                }
            }

            foreach (var stairway in _building.Stairways)
            {
                if (Equals(stairway, place)) continue;

                bool isCollide = false;

                var px2 = stairway.PointsX;
                var py2 = stairway.PointsY;

                for (int i = 0; i < px1.Count; ++i)
                {
                    double x = px1[i];
                    double y = py1[i];

                    if (Helper.IsCollide(x, y, px2, py2)) isCollide = true;

                    x = px2[i];
                    y = py2[i];

                    if (Helper.IsCollide(x, y, px1, py1)) isCollide = true;
                }


                if (isCollide)
                {
                    place.Collide();
                    stairway.Collide();
                    if (!stairway.Collisions.Contains(place))
                        stairway.Collisions.Add(place);

                    isInterscept = true;
                }
                else
                {
                    if (stairway.Collisions.Contains(place))
                        stairway.Collisions.Remove(place);

                    if (stairway.Collisions.Count == 0)
                        stairway.NonCollide();
                }
            }

            if (!isInterscept) place.NonCollide();
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

        private void MoveSelected(double x, double y)
        {
            _isChanged = true;

            double dX = Helper.Tan(x, _lastPosX);
            double dY = Helper.Tan(y, _lastPosY);

            dX /= _scale.ScaleX;
            dY /= _scale.ScaleY;

            double moveX = 0;
            double moveY = 0;

            if (x > _lastPosX) moveX += dX;
            else moveX -= dX;

            if (y > _lastPosY) moveY += dY;
            else moveY -= dY;

            _lastPosX = x;
            _lastPosY = y;

            if (_mode == CanvasMode.Edit)
            {
                if (_selectedItem == null) return;
                var place = _selectedItem as Place;
                if (place == null) return;
                double _x = x - _translation.X;
                double _y = y - _translation.Y;
                foreach (var obstacle in place.Obstacles.Where(p => Helper.IsCollide(_x, _y, p.PointsX, p.PointsY)))
                {
                    MoveObstacle(moveX, moveY, obstacle);
                }
            }
            else
            {
                foreach (var v in _selectedItems)
                {
                    switch (v.Type)
                    {
                        case Entity.EntityType.Portal:
                            MovePortal(moveX, moveY, v);
                            break;
                        default:
                            MovePlace(moveX, moveY, v);
                            break;
                    }
                }

                if (_selectedItem != null)
                {
                    switch (_selectedItem.Type)
                    {
                        case Entity.EntityType.Portal:
                            MovePortal(moveX, moveY, _selectedItem);
                            break;
                        default:
                            MovePlace(moveX, moveY, _selectedItem);
                            break;
                    }
                }
            }
        }

        #endregion

        private void ChangeStageName()
        {
            int s = _curStage + 1;                            
            Stage.Text = "Этаж " + s;
        }
        
        private void SetHidden()
        {
            if (_building.Places.Count > _curStage)
            {
                foreach (var v in _building.Places[_curStage])
                {
                    v.Hide();
                    v.HideObstacles();
                }
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var v in _building.Portals[_curStage])
                 v.Hide();
            }

            int s = _curStage + 1;
            foreach (var v in _building.Stairways)
            {
                if (s >= v.StageFrom && s <= v.StageTo) v.Show();
                else v.Hide();
            }
        }

        private void SetVisible()
        {
            if (_building.Places.Count > _curStage)
            {
                foreach (var v in _building.Places[_curStage])
                {
                    v.Show();
                    v.ShowObstacles();
                }
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var v in _building.Portals[_curStage])
                    v.Show();
            }
            
            int s = _curStage + 1;
            foreach (var v in _building.Stairways)
            {
                if (s >= v.StageFrom && s <= v.StageTo)
                    v.Show();
                else
                    v.Hide();
            }
        }

        private bool LoadBinary(string fileName)
        {
            _building = new Building();
            var lf = new Helpers.IO.LoadFile(fileName, _building);
            if (lf.Load())
            {
                _building = lf.Building;
                return true;
            }
            return false;
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
        
        private void DrawPlan()
        {
            if (_building.Places.Count > _curStage)
                foreach (var p in _building.Places[_curStage])
                    p.Show();

            if (_building.Portals.Count > _curStage)
                foreach (var p in _building.Portals[_curStage])
                    p.Show();

            int s = _curStage + 1;
            foreach (var v in _building.Stairways)
            {
                if (s >= v.StageFrom && s <= v.StageTo)
                    v.Show();
                else
                    v.Hide();
            }
        }
        
        private void DrawGrid()
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
        
        private Entity SelectObject(double x, double y)
        {
            if (_selectedItem != null)
            {
                _selectedItem.Deselect();
                _selectedItem = null;
            }

            Entity entity = null;
            if (_building.Places.Count > _curStage)
            {
                foreach (var p in _building.Places[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                {
                    entity = p;
                }
            }

            foreach (var p in _building.Stairways.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
            {
                entity = p;
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var p in _building.Portals[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                {
                    entity = p;
                }
            }

            return entity;
        }

        private void SelectObjects(double x, double y)
        {
            if (_building.Places.Count > _curStage)
            {
                foreach (var p in _building.Places[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                {
                    if (_selectedItems.Contains(p))
                    {
                        _selectedItems.Remove(p);
                        p.Deselect();
                    }
                    else
                    {
                        _selectedItems.Add(p);
                        p.Select();
                    }
                }
            }

            foreach (var p in _building.Stairways.Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
            {
                if (_selectedItems.Contains(p))
                {
                    _selectedItems.Remove(p);
                    p.Deselect();
                }
                else
                {
                    _selectedItems.Add(p);
                    p.Select();
                }
            }

            if (_building.Portals.Count > _curStage)
            {
                foreach (var p in _building.Portals[_curStage].Where(p => Helper.IsCollide(x, y, p.PointsX, p.PointsY)))
                {
                    if (_selectedItems.Contains(p))
                    {
                        _selectedItems.Remove(p);
                        p.Deselect();
                    }
                    else
                    {
                        _selectedItems.Add(p);
                        p.Select();
                    }
                }
            }
        }
        
        private void DeselectAll()
        {
            foreach (var v in _selectedItems)
                v.Deselect();

            _selectedItems.Clear();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (_isChanged)
            {
                var mb = MessageBox.Show("Имеются не сохраненные данные, сохранить?", "Подтверждение", MessageBoxButton.YesNo);
                if (mb == MessageBoxResult.Yes) SaveProject();
            }
        }

        private void SetHiddenStairways()
        {
            foreach (var stairway in _building.Stairways)
            {
                stairway.UI.Visibility = Visibility.Hidden;
            }
        }

        private void SetVisibleStairways()
        {
            foreach (var stairway in _building.Stairways)
            {
                stairway.UI.Visibility = Visibility.Visible;
            }
        }
       
        private void ActiveDeactiveMenu(bool isEnabled)
        {
            SelectTool.IsEnabled = isEnabled;
            MoveTool.IsEnabled = isEnabled;
            StageNext.IsEnabled = isEnabled;
            StageTo.IsEnabled = isEnabled;
            AddPlace.IsEnabled = isEnabled;
            AddDoor.IsEnabled = isEnabled;
        }

        private void Click_CheckEntity(object sender, RoutedEventArgs e)
        {
            if (_selectedItem == null) return;
            
            _selectedItems.Clear();

            switch (_selectedItem.Type)
            {
                case Entity.EntityType.Portal:
                    var portal = _selectedItem as Portal;
                    if (portal != null)
                    {
                        if (portal.RoomA != null)
                        {
                            _selectedItems.Add(portal.RoomA);
                            portal.RoomA.Select();
                        }

                        if (portal.RoomB != null)
                        {
                            _selectedItems.Add(portal.RoomB);
                            portal.RoomB.Select();
                        }
                    }
                    break;
                default:
                    var place = _selectedItem as Place;
                    if (place != null)
                    {
                        if (_building.Portals.Count > _curStage)
                        {
                            foreach (var p in _building.Portals[_curStage])
                            {
                                if (p.RoomA != null && place.Equals(p.RoomA))
                                {
                                    _selectedItems.Add(p);
                                    p.Select();
                                }
                                if (p.RoomB != null && place.Equals(p.RoomB))
                                {
                                    _selectedItems.Add(p);
                                    p.Select();
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }    
}
