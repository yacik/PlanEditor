﻿using System;
using System.Collections.Generic;
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
        
        public decimal StageFrom { get; set; }
        public decimal StageTo { get; set; }
        
        [NonSerialized] public List<Cell> StartPoints = new List<Cell>();
        [NonSerialized] public List<Cell> EndPoints = new List<Cell>();
        [NonSerialized] public List<Cell> Cells;
    }
}
