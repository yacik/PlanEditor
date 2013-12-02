
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace PlanEditor.Entities
{
    [Serializable]
    public class QRPointer
    {
        public QRPointer(string name)
        {
            Code = name + "generate code";
        }

        [DataMember] public int ID { get; set; }
        [DataMember] public string Code { get; private set; }
        [DataMember] public Point Pos { get; set; }
    }
}
