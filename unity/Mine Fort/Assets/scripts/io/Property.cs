using Rimworld.model.components;
using System.Runtime.Serialization;

namespace Rimworld.model.io
{
    [DataContract]
    public class Property : TagObject
    {
        public Property(string name)
        {
            this.name = name;
            randomValue = true;
        }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public GameValue value { get; set; }


        public bool randomValue { get; set; }

        public GameValue howMuchToSpawn { get; set; }

        internal int CalcQtyToSpawn()
        {
            if (howMuchToSpawn == null) return 1;
            return Utils.Random(howMuchToSpawn.minValue, howMuchToSpawn.maxValue);
        }
    }
}
