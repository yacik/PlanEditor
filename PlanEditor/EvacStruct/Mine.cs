using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.EvacStruct
{
    [DataContract]
    public class Mine
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
}
