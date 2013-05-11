using PlanEditor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor
{
    public class Data
    {
        public static readonly  double GridStep = 17.611;   // Расчет Шаг сетки, м Hxy        
        public static readonly double Sigma = 0.02839;      // При условии выставленной сетки 0.5 метров     
    }
}