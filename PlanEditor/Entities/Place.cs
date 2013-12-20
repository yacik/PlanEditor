using System;
using System.Collections.Generic;
using System.Text;
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
            ScanarioID = -1;
            FireType = 0;
        }

        public int CountNodes { get; set; }	        // Количество узлов сетки, принадлежащих помещению 
        public int Ppl { get; set; }	            // Количество людей в помещении (фактическое наличие)
        public int MaxPeople { get; set; }	        // Максимальное количество людей в помещении	                
        public string Name { get; set; } 	        // Название помещения        
        public double Height { get; set; }		    // высота помещения, м        
        public double SHeigthRoom { get; set; }     // высота  площадки,  на  которой  находятся  люди, над полом, м
        public double DifHRoom { get; set; }	    // разность высот пола, равная нулю при горизонтальном его расположении, м	
        public double EvacWide { get; set; }
        public int MainType { get; set; }
        public int SubType { get; set; }        
        public bool IsMovable { get; set; }

        public int FireType { get; set; }
        public int ScanarioID { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
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

        public void HideObstacles()
        {
            if (Obstacles != null)
            {
                foreach (var obstacle in Obstacles)
                {
                    obstacle.Hide();
                }
            }
        }

        public void ShowObstacles()
        {
            if (Obstacles != null)
            {
                foreach (var obstacle in Obstacles)
                {
                    obstacle.Show();
                }
            }
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

        [NonSerialized]
        private bool _isCollide;

        public bool IsCollide
        {
            get { return _isCollide; }
        } 
        
        public void Collide()
        {
            UI.Fill = Colours.Red;
            _isCollide = true;
        }

        public void NonCollide()
        {
            switch (Type)
            {
                case EntityType.Place:
                    UI.Fill = Colours.Indigo;
                    break;
                case EntityType.Halfway:
                    UI.Fill = Colours.Green;
                    break;
                case EntityType.Stairway:
                    UI.Fill = Colours.Violet;
                    break;
                case EntityType.Portal:
                    UI.Fill = Colours.LightGray;
                    break;
            }
            _isCollide = false;
        }

        public List<Obstacle> Obstacles = new List<Obstacle>();
    }
}
