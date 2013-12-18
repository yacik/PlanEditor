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
using PlanEditor.EvacStruct;

namespace PlanEditor.Helpers.IO
{
    public class SaveToEvac
    {
        private static readonly List<Entity> Entities = new List<Entity>();
        private static readonly List<List<Stairway>> Stairways = new List<List<Stairway>>();
        private static EvacStruct.Building _building;
        private static Entities.Building _dBuilding;

        public static void Save(string fileName, RegGrid.Grid grid, Entities.Building building)
        {
            _building = new EvacStruct.Building(building);
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

                    int id = GetIdByEntity(stair);
                    var room = new Room(stair, id, i, null);
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

                            ownerId = GetIdByEntity(cell.Owner);
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
                        var room = new Room(place, id, curStage, GetIdByEntity(place));
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
                        var portal = new Door(p, id, curStage, GetIdByEntity(p.RoomA));
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
                    for (int curStage = 0; curStage < building.Stages; ++curStage)
                    {
                        if (building.Places.Count > curStage)
                        { 
                            foreach (var place in building.Places[curStage])
                            {
                                if (s.Key == place.ID)
                                {
                                    var room = _building.Rooms.SingleOrDefault(r => r.parent == place);
                                    if (!tm.Keys.Contains(room.ID))
                                    {
                                        tm.Add(room.ID, s.Value);
                                    }
                                }                                
                            }
                        }
                    }
                }

                _building.Times.Add(scenario.Key, tm);
            }
            
            #endregion

            #region Save
            //Сохраняем для расчетов эвакуации
            var serializer = new JsonSerializer();
            using (var sw = new StreamWriter(fileName))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, _building);
                }
            }

            // Сохраняем изображения
            var folder = Path.GetDirectoryName(fileName);
            SaveImage.SaveFile(_building, folder);

            #endregion

            
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
            return _building.Rooms.FirstOrDefault(r => r.ID == id);            
        }

        private static int GetIdByEntity(Entity entity)
        {
            for (int i = 0; i < Entities.Count; ++i)
            {
                if (Entities[i] == entity) return i;
            }

            Entities.Add(entity);            
            
            return Entities.Count - 1;
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
    }
}