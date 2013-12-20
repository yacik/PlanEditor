using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.EvacStruct
{
    [DataContract]
    public class Building
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
            Times = new Dictionary<int, Dictionary<int, double>>();
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
        [DataMember] public Dictionary<int, Dictionary<int, double>> Times;
        [DataMember] public int OuterDoors;
        [DataMember] public int InnerDoors;
        [DataMember] public double Wide;
        [DataMember] public double Length;
        [DataMember] public double Height;
    }
}
