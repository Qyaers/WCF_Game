using System.ServiceModel;

namespace wcf_chat1
{
    public class ServerUser
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string MSG { get; set; }

        public OperationContext operationContext { get; set; }
    }
}
