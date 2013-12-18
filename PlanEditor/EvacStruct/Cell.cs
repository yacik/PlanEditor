using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor.EvacStruct
{
    [DataContract]
    public class Cell
    {
        [DataMember] public int m;
        [DataMember] public int n;
        [DataMember] public int k;
        [DataMember] public int code;
        [DataMember] public int ParentID;
    }
}
