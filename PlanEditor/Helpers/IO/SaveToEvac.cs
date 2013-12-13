using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using Newtonsoft.Json;
using PlanEditor.Entities;

namespace PlanEditor.Helpers.IO
{
    #region Export DS

    [DataContract]
    internal struct Room
    {
        public Room(Place place, int id, double z)
        {
            ID = id;
            parent = place; 
            CountNodes = place.CountNodes;
            Ppl = place.Ppl;
            MaxPeople = place.MaxPeople;
            Name = place.Name;
            Wide = place.Wide;
            Height = place.Height;
            Length = place.Length;
            SHeigthRoom = place.SHeigthRoom;
            DifHRoom = place.DifHRoom;
            MainType = place.MainType;
            SubType = place.SubType;
            scenario = place.FireType;
            //timeblock = (scenario == 1) ? 95.00 : 0.00;
            timeblock = 0.00;
            
            x1 = place.PointsX[0];
            y1 = place.PointsY[0];
            z1 = z;
            x2 = place.PointsX[1];
            y2 = place.PointsY[2];
            z2 = Height; // meters

            Neigh = new List<int>();
        }

        public void SetScenario(int id)
        {
            scenario = id;
        }

        [DataMember] public int ID;
        [DataMember] public int CountNodes;
        [DataMember] public int Ppl;
        [DataMember] public int MaxPeople;
        [DataMember] public string Name;
        [DataMember] public double Wide;
        [DataMember] public double Height;
        [DataMember] public double Length;
        [DataMember] public double SHeigthRoom;
        [DataMember] public double DifHRoom;
        [DataMember] public int MainType;
        [DataMember] public int SubType;
        [DataMember] public List<int> Neigh;
        [DataMember] public double timeblock;
        [DataMember] public int scenario;
        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;

        public Place parent;
    };

    [DataContract]
    internal struct Building
    {
        public Building(Entities.Building building)
        {
            Name = building.Name;
            Stages = building.Stages;
            NumPlaces = building.GetNumPlaces;
            NumMines = building.GetNumMines;
            NumNodes = building.NumNodes;
            MaxPeople = building.MaxPeople;
            People = building.People;
            PplCell = building.PplCell;
            Lx = building.Lx;
            Ly = building.Ly;
            Rooms = new List<Room>();
            Portals = new List<Door>();
            Grid = new List<Cell>();
            Mines = new List<Mine>();
            Times = new Dictionary<string, Dictionary<int, double>>();
            Nx = building.Row;
            Ny = building.Col;

            OuterDoors = 0;
            InnerDoors = 0;
            foreach (var portals in building.Portals)
            {
                foreach (var portal in portals)
                {
                    if (portal.RoomA == null || portal.RoomB == null)
                        ++OuterDoors;
                    if (portal.RoomA != null && portal.RoomB != null)
                        ++InnerDoors;
                }
            }

            Wide = MyMath.Helper.MetersToPxl(Lx);
            Length = MyMath.Helper.MetersToPxl(Ly);
            Height = building.HeightStage; // meters
        }

        [DataMember] public string Name;
        [DataMember] public int Stages;
        [DataMember] public int NumPlaces;
        [DataMember] public int NumMines;
        [DataMember] public int NumNodes;
        [DataMember] public int MaxPeople;
        [DataMember] public int People;
        [DataMember] public int PplCell;
        [DataMember] public double Lx;
        [DataMember] public double Ly;
        [DataMember] public int Nx;
        [DataMember] public int Ny;
        [DataMember] public List<Room> Rooms;
        [DataMember] public List<Door> Portals;
        [DataMember] public List<Cell> Grid;
        [DataMember] public List<Mine> Mines;
        [DataMember] public Dictionary<string, Dictionary<int, double>> Times;
        [DataMember] public int OuterDoors;
        [DataMember] public int InnerDoors;
        [DataMember] public double Wide;
        [DataMember] public double Length;
        [DataMember] public double Height;
    };

    [DataContract]
    internal struct Door
    {
        public Door(Portal portal, int id, int stage)
        {
            Wide = portal.Wide;
            Code = (portal.RoomB == null) ? 1 : 2;
            ID = id;
            Cells = new List<int>();
            if (portal.Cells != null)
            {
                foreach (var cell in portal.Cells)
                {
                    Cells.Add(cell.M);
                    Cells.Add(cell.N);
                    Cells.Add(cell.K);
                }
            }
            // x - 1, y - 2
            directAper = (portal.Orientation == Portal.PortalOrient.Vertical) ? 1 : 2;

            x1 = portal.PointsX[0];
            y1 = portal.PointsY[0];
            x2 = portal.PointsX[1];
            y2 = portal.PointsY[2];
            z1 = stage;
            z2 = (portal.Height < 1) ? 3.0 : portal.Height; // meters
        }

        [DataMember] public double Wide;
        [DataMember] public int Code;
        [DataMember] public int ID;
        [DataMember] public List<int> Cells;
        [DataMember] public int directAper;

        [DataMember] public double x1;
        [DataMember] public double y1;
        [DataMember] public double z1;
        [DataMember] public double x2;
        [DataMember] public double y2;
        [DataMember] public double z2;
    }

    [DataContract]
    internal struct Mine
    {
        public Mine(int from, int to, RegGrid.Cell startCell, RegGrid.Cell endCell)
        {
            StageFrom = from - 1;
            StageTo = to - 1;
            ID = _ID;
            ++_ID;
            StartPoints = new[] { startCell.M, startCell.N };
            EndPoints = new[] { endCell.M, endCell.N };
        }

        [DataMember] public decimal StageFrom;
        [DataMember] public decimal StageTo;
        [DataMember] public int ID;
        [DataMember] public int[] StartPoints;
        [DataMember] public int[] EndPoints;

        private static int _ID = 1000;
    }

    [DataContract]
    internal struct Cell
    {
        [DataMember] public int m;
        [DataMember] public int n;
        [DataMember] public int k;
        [DataMember] public int code;
        [DataMember] public int ParentID;
    }
    
    #endregion

    public class SaveToEvac
    {
        private static readonly List<Entity> Entities = new List<Entity>();
        private static readonly List<List<Stairway>> Stairways = new List<List<Stairway>>();
        private static Building _building;
        private static Entities.Building _dBuilding;

        public static void Save(string fileName, RegGrid.Grid grid, Entities.Building building)
        {
            _building = new Building(building);
            _dBuilding = building;
            
            Entities.Clear();
            Stairways.Clear();

            // Создаем список лестниц для последующий обработки (в зависимости от кол-ва этажей)
            // потом для каждой лестнице на этаже определяем ячейки
            for (int i = 0; i < building.Stages; ++i)
            {
                Stairways.Add(new List<Stairway>());
            }

            foreach (var strws in building.Stairways)
            {
                int stageFrom = strws.StageFrom - 1;
                int stageTo = strws.StageTo;
                for (int i = stageFrom; i < stageTo; ++i, ++stageFrom)
                {
                    var stair = new Stairway
                    {
                        CountNodes = strws.CountNodes,
                        Ppl = strws.Ppl,
                        MaxPeople = strws.MaxPeople,
                        Name = strws.Name,
                        Height = strws.Height,
                        SHeigthRoom = strws.SHeigthRoom,
                        DifHRoom = strws.DifHRoom,
                        EvacWide = strws.EvacWide,
                        MainType = strws.MainType,
                        SubType = strws.SubType,
                        UI = strws.UI,
                        StageFrom = stageFrom + 1,
                    };

                    Stairways[i].Add(stair);

                    int id = GenerateId(stair);
                    var room = new Room(stair, id, i);
                    _building.Rooms.Add(room);
                }
            }

            RegGrid.RecognizeGrid.DefineStairways(Stairways, grid); // распознаем ячейки на лестницах

            /*for (int i = 0; i < building.Stages; ++i)
            {
                Debug.WriteLine(i);

                foreach (var stairway in Stairways[i])
                {
                    stairway.Print();
                }
            }*/

            for (int i = 0; i < building.Stages - 1; ++i)
            {
                int stage = i + 1;
                int node = (i % 2 == 0) ? 1 : 2;
                foreach (var stairway in Stairways[i])
                {
                    if (stairway.StageFrom == i) continue;

                    var start = stairway.Cells[node];
                    RegGrid.Cell end = null;

                    foreach (var nextStair in Stairways[i + 1].AsParallel().Where(s => s.UI.Equals(stairway.UI)))
                    {
                        end = nextStair.Cells[node];
                    }

                    if (end != null)
                    {
                        var mine = new Mine(stage, stage + 1, start, end);
                        _building.Mines.Add(mine);
                    }
                }
            }

            /*foreach (var mine in _building.Mines)
            {
                Debug.WriteLine(mine.StageFrom + " " + mine.StageTo + " [" + mine.StartPoints[0] + ", " + mine.StartPoints[1] + "] [" + mine.EndPoints[0] + ", " + mine.EndPoints[1] + "]");
            }//*/

            for (int curStage = 0; curStage < building.Stages; ++curStage)
            {
                
                #region grid
                
                  //-1; // outside of computational domain
                  //+1; // внешняя дверь (выход из опасной зоны)
                  //+2; // внутренняя дверь
                  //+5; // внутренняя ячейка горизонтальной поверхности
                  //+6; // внутренняя ячейка горизонтальной поверхности cо ступеньками
                  //+7; // внутренняя ячейка шахты (не принадлежит слою и не имеет соседей в горизонтальной плоскости)
                  //+9; // код узла шахты не подлежащий расчету эвакуации (проекция на уровень layer лестничного проема
                      //10; // код узла лестничного перехода, относящийся к шахте >=10 (узел принадлежит уровню layer)
                
                if (grid.Cells.Count > curStage)
                {
                    foreach (var cell in grid.Cells[curStage])
                    {
                        int code = -1;
                        int ownerId = -1;
                        if (cell.Owner != null)
                        {
                            switch (cell.Owner.Type)
                            {
                                case Entity.EntityType.Place:
                                case Entity.EntityType.Halfway:
                                case Entity.EntityType.Stairway:
                                    var place = cell.Owner as Place;
                                    if (place == null) continue;

                                    if (place.Obstacles.Count == 0)
                                    {
                                        code = 5;
                                    }
                                    else
                                    {
                                        foreach (var obst in place.Obstacles)
                                        {
                                            code = MyMath.Helper.IsCollide(cell.CenterX, cell.CenterY, obst.PointsX, obst.PointsY) ? 9 : 5;

                                            if (code == 9) break;
                                        }
                                    }
                                    break;
                                case Entity.EntityType.Portal:
                                    var portal = cell.Owner as Portal;
                                    if (portal != null) code = (portal.RoomA != null && portal.RoomB != null) ? 2 : 1;
                                    
                                    break;
                            }

                            ownerId = GenerateId(cell.Owner);
                        }

                        _building.Grid.Add(new Cell { m = cell.M,n = cell.N,code = code,ParentID = ownerId,k = cell.K });
                    }
                }
                #endregion
                
                #region places
                if (building.Places.Count > curStage)
                {
                    foreach (var place in building.Places[curStage])
                    {
                        int id = GetIdByEntity(place);
                        var room = new Room(place, id, curStage);
                        _building.Rooms.Add(room);
                    }

                    foreach (var place in building.Places[curStage])
                    {
                        int id = GetIdByEntity(place);
                        foreach (var room in _building.Rooms)
                        {
                            if (room.ID != id) continue;

                            foreach (var portal in building.Portals[curStage])
                            {
                                if (portal.RoomA == place && portal.RoomB != null) DefineStairway(portal.RoomB, room, curStage);
                                else if (portal.RoomB == place && portal.RoomA != null) DefineStairway(portal.RoomA, room, curStage);
                            }
                        }
                    }
                }
                #endregion

                #region stairways

                foreach (var p in building.Portals[curStage])
                {
                    if (p.RoomA == null || p.RoomB == null) continue;

                    Place place = null;
                    Place stair = null;

                    if (p.RoomA.Type == Entity.EntityType.Stairway)
                    {
                        place = p.RoomB;
                        stair = p.RoomA;
                    }
                    else if (p.RoomB.Type == Entity.EntityType.Stairway)
                    {
                        place = p.RoomA;
                        stair = p.RoomB;
                    }

                    if (place == null || stair == null) continue;

                    int idR = GetIdByEntity(place);
                    foreach (var s in Stairways[curStage])
                    {
                        if (s.UI.Equals(stair.UI))
                        {
                            int id = GetIdByEntity(s);
                            foreach (var r in _building.Rooms)
                            {
                                if (r.ID != id) continue;
                                r.Neigh.Add(idR);
                            }
                        }
                    }
                }
                #endregion

                #region portals
                
                if (building.Portals.Count > curStage)
                {
                    foreach (var p in building.Portals[curStage])
                    {
                        int id = GetIdByEntity(p);
                        var portal = new Door(p, id, curStage);
                        _building.Portals.Add(portal);
                    }
                }

                #endregion
            }

            _building.NumMines = _building.Mines.Count;
            _building.NumPlaces += Stairways.Sum(sum => sum.Count);
            
            DefineScenarion(_building.Rooms);


            #region Scenarios
            var fs = new StreamReader("timeblock.json");
            string json = fs.ReadToEnd();
            fs.Close();

            var time = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, double>>>(json);

            // т.к. ID для эвакуации не совпадают с ID для пожара и редактора, нужно преобразовать
            foreach (var scenario in time)
            {
                var tm = new Dictionary<int, double>();

                foreach (var s in scenario.Value)
                {
                    int countPlaces = 0;
                    for (int curStage = 0; curStage < building.Stages; ++curStage)
                    {
                        if (building.Places.Count > curStage)
                        {
                            foreach (var place in building.Places[curStage])
                            {
                                if (s.Key == countPlaces)
                                {
                                    var room = _building.Rooms.SingleOrDefault(r => r.parent == place);
                                    if (!tm.Keys.Contains(room.ID)) 
                                        tm.Add(room.ID, s.Value);
                                }
                                ++countPlaces;
                            }
                        }
                    }

                }

                _building.Times.Add(scenario.Key, tm);
            }
            
            #endregion

            #region Save
            
            var serializer = new JsonSerializer();
            using (var sw = new StreamWriter(fileName))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, _building);
                }
            }


            #endregion

            //var folder = Path.GetDirectoryName(fileName);
            //SaveImage.SaveFile(_building, folder);
        }

        private static void DefineScenarion(List<Room> rooms)
        {
            foreach (var place in rooms.Where(r => r.scenario == 1))
            {
                //Debug.Write("* ");
                //DebugPrintIDOfParentByChild(place);
                foreach (var n in place.Neigh)
                {
                    var neigh = GetRoomById(n);
                    neigh.SetScenario(place.ID);
                    //DebugPrintIDOfParentByChild(neigh);
                }
            }
        }

        /*private static void DebugPrintIDOfParentByChild(Room room)
        {
            int count = 0;
            for (int i = 0; i < _dBuilding.Stages; ++i)
            {
                foreach (var parent in _dBuilding.Places[i])
                {
                    if (parent.Equals(room.parent))
                    {
                        Debug.WriteLine(count);
                    }
                    ++count;
                }
            }
        }*/

        private static Room GetRoomById(int id)
        {
            var room = new Room();

            foreach (var r in _building.Rooms.Where(r => r.ID == id))
            {
                room = r;
                break;
            }

            return room;
        }

        private static void DefineStairway(Place place, Room room, int curStage)
        {
            if (place.Type == Entity.EntityType.Stairway)
            {
                foreach (var stairway in Stairways[curStage])
                {
                    if (stairway.UI.Equals(place.UI))
                    {
                        int idR = GetIdByEntity(stairway);
                        if (idR != -1) room.Neigh.Add(idR);
                    }
                }
            }
            else
            {
                int idR = GetIdByEntity(place);
                if (idR != -1) room.Neigh.Add(idR);
            }
        }

        private static int GetIdByEntity(Entity entity)
        {
            for (int i = 0; i < Entities.Count; ++i)
            {
                if (Entities[i] == entity) return i;
            }
            return -1;
        }

        private static int GenerateId(Entity entity)
        {
            for (int i = 0; i < Entities.Count; ++i)
            {
                if (Entities[i] == entity) return i;
            }
            Entities.Add(entity);
            return Entities.Count - 1;
        }
    }
}