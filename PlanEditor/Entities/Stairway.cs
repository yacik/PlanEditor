using System;
using System.Collections.Generic;
using System.Diagnostics;
using PlanEditor.RegGrid;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Stairway : Place
    {
        public Stairway()
        {
            Type = EntityType.Stairway;
        }
        
        public int StageFrom { get; set; }
        public int StageTo { get; set; }
                
        [NonSerialized] public List<Cell> Cells = new List<Cell>();

        public void Print()
        {
            Debug.Write("StageFrom " + StageFrom + ", StageTo " + StageTo + ", ");
            foreach (var cell in Cells)
            {
                Debug.Write("[" + cell.M + ", " + cell.N + "]");
            }
            Debug.WriteLine("\n");
        }
    }
}
