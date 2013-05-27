using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using PlanEditor.Entities;

namespace PlanEditor.Helpers.IO
{
    #region Export DS

    [DataContract]
    internal struct Room
    {
        public Room(Place place)
        {
            ID = place.ID;
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

            Neigh = new List<int>();
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
        [DataMember] public int OuterDoors;
        [DataMember] public int InnerDoors;
    };

    [DataContract]
    internal struct Door
    {
        public Door(Portal portal)
        {
            Wide = portal.Wide;
            Code = (portal.RoomA == null && portal.RoomB == null) ? 2 : 1;
            ID = portal.ID;
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
        }

        [DataMember] public double Wide;
        [DataMember] public int Code;
        [DataMember] public int ID;
        [DataMember] public List<int> Cells;
    }

    [DataContract]
    internal struct Mine
    {
        public Mine(int from, int to, RegGrid.Cell startCell, RegGrid.Cell endCell)
        {
            StageFrom = from;
            StageTo = to;
            ID = _ID;
            ++_ID;
            StartPoints = new [] { startCell.M, startCell.N };
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
        public static void Save(string fileName, RegGrid.Grid grid, Entities.Building building)
        {
            var sw = new StreamWriter(fileName);
            var _building = new Building(building);

            for (int i = 0; i < building.Stages; ++i)
            {
                /*
                    -1; // outside of computational domain
                    +1; // внешняя дверь (выход из опасной зоны)
                    +2; // внутренняя дверь
                    +5; // внутренняя ячейка горизонтальной поверхности
                    +6; // внутренняя ячейка горизонтальной поверхности cо
		                    // ступеньками
                    +7; // внутренняя ячейка шахты (не принадлежит слою и не
		                    // имеет соседей в горизонтальной плоскости)
                    +9; // код узла шахты не подлежащий расчету эвакуации
		                    // (проекция на уровень layer лестничного проема
	                10; // код узла лестничного перехода, относящийся к шахте
						                    // >=10 (узел принадлежит уровню layer)
                */
                if (grid.Cells.Count > i)
                {
                    foreach (var cell in grid.Cells[i])
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
                                    code = 5;
                                    break;
                                case Entity.EntityType.Portal:
                                    var portal = cell.Owner as Portal;
                                    if (portal != null) code = (portal.RoomA != null && portal.RoomB != null) ? 2 : 1;
                                    break;
                            }
                            ownerId = cell.Owner.ID;
                        }

                        _building.Grid.Add(new Cell {m = cell.M,n = cell.N,code = code,ParentID = ownerId,k = cell.K});
                    }
                }

                if (building.Places.Count > i)
                {
                    foreach (var place in building.Places[i])
                    {
                        var room = new Room(place);

                        foreach (var portal in building.Portals[i])
                        {
                            if (portal.RoomA == place)
                            {
                                if (portal.RoomB != null)
                                    room.Neigh.Add(portal.RoomB.ID);
                            }
                            else if (portal.RoomB == place)
                            {
                                if (portal.RoomA != null)
                                    room.Neigh.Add(portal.RoomA.ID);
                            }
                        }
                        _building.Rooms.Add(room);
                    }
                }
               
                if (building.Portals.Count > i)
                {
                    foreach (var p in building.Portals[i])
                    {
                        var portal = new Door(p);
                        _building.Portals.Add(portal);
                    }
                }
            }

            foreach (var v in building.Mines)
            {
                int start = (int)v.StageFrom - 1;
                DefineMineEnter(v, building.Portals[start], true);

                int end = (int)v.StageTo - 1;
                DefineMineEnter(v, building.Portals[end], false);

                for (int i = 0; i < v.StartPoints.Count; ++i)
                {
                    if (i%2 != 0) continue;

                    var mine = new Mine((int)v.StageFrom, (int)v.StageTo, v.StartPoints[i], v.EndPoints[i]);
                    _building.Mines.Add(mine);
                }
            }
            _building.NumMines = _building.Mines.Count;
        
            var stream = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(List<Building>));
            ser.WriteObject(stream, _building);
            stream.Position = 0;
            var sr = new StreamReader(stream);
            string read = sr.ReadToEnd();
            sw.Write(read);
            sw.Close();
        }

        private static void DefineMineEnter(Stairway mine, List<Portal> portals, bool isFirst)
        {
            if (mine.EndPoints == null) mine.EndPoints = new List<RegGrid.Cell>();
            if (mine.StartPoints == null) mine.StartPoints = new List<RegGrid.Cell>();
            
            foreach (var portal in portals)
            {
                if (portal.RoomA == null || portal.RoomB == null) continue;
                if (portal.RoomA == mine || portal.RoomB == mine)
                {
                    var isHor = (portal.Orientation == Portal.PortalOrient.Horizontal);
                    foreach (var cell in portal.Cells)
                    {
                        //int pos = (isHor) ? cell.N : cell.M;
                        int index = (isHor) ? cell.M : cell.N;

                        int min = int.MaxValue;
                        int max = int.MinValue;

                        foreach (var mc in mine.Cells)
                        {
                            if (isHor)
                            {
                                if (mc.N < min) min = mc.N;
                                if (mc.N > max) max = mc.N;
                            }
                            else
                            {
                                if (mc.M < min) min = mc.M;
                                if (mc.M > max) max = mc.M;
                            }
                        }
                        
                        int point = ((max - min)/2) + min;

                        foreach (var mc in mine.Cells)
                        
                        {
                            if (isHor)
                            {
                                if (mc.M == index && mc.N == point)
                                {
                                    if (isFirst) mine.StartPoints.Add(mc);
                                    else mine.EndPoints.Add(mc);
                                }
                            }
                            else
                            {
                                if (mc.N == index && mc.M == point)
                                {
                                    if (isFirst) mine.StartPoints.Add(mc);
                                    else mine.EndPoints.Add(mc);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
