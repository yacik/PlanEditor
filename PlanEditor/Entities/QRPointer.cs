
namespace PlanEditor.Entities
{
    public class QRPointer
    {
        public QRPointer(string name)
        {
            Code = name + "generate code";
        }

        public int ID { get; set; }
        public string Code { get; private set; }
    }
}
