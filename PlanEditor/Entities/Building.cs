﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Building
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Functionality { get; set; }
        public int People { get; set; }
        public int MaxPeople { get; set; }
        public int FireSafetySys { get; set; } // 1 - 3
        public int FireSignal { get; set; } // 1 - 3
        public int Notification { get; set; } // 1 - 3
        public int AntiFog { get; set; } // 1 - 3
        public string Insurance { get; set; }
        public int Stages { get; set; }
        public int NumNodes {get; set; }    // Характерное число узлов (входной параметр)         
        public int Row { get; set; }
        public int Col { get; set; }
        public double xMax { get; set; }
        public double yMax { get; set; }
        public double Lx { get; set; }      // Размеры проекции здания по Х, метры 
        public double Ly { get; set; }      // Размеры проекции здания по Y, метры       
        public int PplCell { get; set; }    // Расчет Количество ячеек сетки в здании, где могут находиться люди 

        public int GetNumPlaces
        {
            get 
            {
                int num = 0;

                foreach (var v in Places)
                    num += v.Count;

                return num;
            }
        }
        public int GetNumPortal
        {
            get 
            {
                int num = 0;
                
                foreach (var v in Portals)
                    num += v.Count;

                return num;
            }
        }
        public int GetNumMines
        {
            get 
            {
                int num = 0;

                foreach (var v in Mines)
                    num += v.Count;

                return num;
            }
        }

        public List<List<Place>> Places = new List<List<Place>>();      // NB кол-во помещений
        public List<List<Place>> Mines = new List<List<Place>>();       // NB кол-во шахт
        public List<List<Portal>> Portals = new List<List<Portal>>();   // NB кол-во порталов

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Name + " ");
            sb.Append(Address + " ");
            sb.Append(Functionality + " ");
            sb.Append(People + " ");
            sb.Append(MaxPeople + " ");
            sb.Append(FireSafetySys + " ");
            sb.Append(FireSignal + " "); 
            sb.Append(Notification + " "); 
            sb.Append(AntiFog + " ");
            sb.Append(Insurance + " ");
            sb.Append(Stages + " ");
            sb.Append(NumNodes + " ");
            sb.Append(Row + " ");
            sb.Append(Col + " ");
            sb.Append(xMax + " ");
            sb.Append(yMax + " ");
            sb.Append(Lx + " ");
            sb.Append(Ly + " ");
            sb.Append(PplCell);

            return sb.ToString();
        }

        public void PrepareForExport()
        {
            for (int i = 0; i < Places.Count; ++i)
            {
                foreach (var v in Places[i])
                {
                    v.PrepareForSave();
                }
            }
        }
    }
}
