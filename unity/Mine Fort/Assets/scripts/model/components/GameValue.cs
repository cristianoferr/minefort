
namespace Rimworld.model.components
{
    public class GameValue
    {


        public GameValue()
        {
            dataType = GameConsts.DATA_TYPE.INT;
            minValue = 0;
            maxValue = 0;
            currValue = 0;
        }

        public GameValue(GameConsts.DATA_TYPE dataType, int min, int max)
        {
            this.dataType = dataType;
            minValue = min;
            maxValue = max;

        }
        public GameConsts.DATA_TYPE dataType { get; set; }
        public float maxValue { get; set; }
        public float minValue { get; set; }
        public object currValue { get; set; }

    }
}
