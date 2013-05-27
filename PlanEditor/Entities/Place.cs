using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using PlanEditor.Helpers;

namespace PlanEditor.Entities
{
    [Serializable]
    public class Place : Entity
    {

        public Place()
        {
            MainType = -1;
            SubType = -1;
            Ppl = 0;
            MaxPeople = 0;
            Name = "";
            IsMovable = true;
        }

        public int CountNodes { get; set; }	        // Количество узлов сетки, принадлежащих помещению 
        public int Ppl { get; set; }	            // Количество людей в помещении (фактическое наличие)
        public int MaxPeople { get; set; }	        // Максимальное количество людей в помещении	                
        public string Name { get; set; } 	        // Название помещения        
        public double Height { get; set; }		    // высота помещения, м        
        public double SHeigthRoom { get; set; }     // высота  площадки,  на  которой  находятся  люди, над полом, м
        public double DifHRoom { get; set; }	    // разность высот пола, равная нулю при горизонтальном его расположении, м	
        public int MainType { get; set; }
        public int SubType { get; set; }

        public bool IsMovable { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(ID + " ");
            sb.Append(CountNodes + " ");
            sb.Append(Ppl + " ");
            sb.Append(MaxPeople + " ");
            sb.Append(Name + " ");          
            sb.Append(Wide + " ");
            sb.Append(Height + " ");
            sb.Append(Length + " ");
            sb.Append(SHeigthRoom + " ");
            sb.Append(DifHRoom + " ");
            sb.Append(MainType + " ");
            sb.Append(SubType + " ");
            
            return sb.ToString();
        }

        public List<Line> Lines 
        { 
            get 
            {
                var lst = new List<Line>();
                var x = PointsX;
                var y = PointsY;
                for (int i = 1, j = 1; i < x.Count && j < y.Count; ++i, ++j)
                {
                    var l = new Line { X1 = x[i - 1], Y1 = y[j - 1], X2 = x[i], Y2 = y[j] };
                    lst.Add(l);                
                }
                
                return lst;
            }         
        }

        [NonSerialized]
        public List<Portal> Exits = new List<Portal>();

        public double Length // длина помещения, м	
        {
            get
            {
                return distance(PointsY[0], PointsY);
            }
        }

        public double Wide	// ширина
        {
            get
            {
                return distance(PointsX[0], PointsX);
            }
        }
    }
}
