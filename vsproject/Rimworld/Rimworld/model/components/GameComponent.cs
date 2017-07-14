
namespace Rimworld.model.components
{
    public class GameComponent : GameEntity
    {

        public GameComponent()
            : base()
        {

        }

        public GameConsts.COMPONENT_TYPE type { get; set; }

        public GameComponent(GameConsts.COMPONENT_TYPE type)
        {
            this.type = type;
        }
        public GameEntity owner { get; set; }

    }
}
