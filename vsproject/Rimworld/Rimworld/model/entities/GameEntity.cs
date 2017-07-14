using Rimworld.model.components;
using System.Collections.Generic;
using System.Linq;

namespace Rimworld.model
{
    //gameentity: qualquer coisa spawnavel.
    public abstract class GameEntity : TagObject
    {
        public GameEntity()
        {
            // components = new List<GameComponent>();
        }

        public virtual void Initialize()
        {

        }

        #region GameValues
        public Dictionary<string, GameValue> values { get; private set; }

        public void AddValue(string name, GameValue value)
        {
            if (values == null) { values = new Dictionary<string, GameValue>(); }
            if (values.ContainsKey(name)) { values.Remove(name); }
            values.Add(name, value);
        }

        public int GetValueAsInt(string name)
        {
            return (int)values[name].currValue;
        }
        #endregion GameValues

        #region Components
        public IList<GameComponent> components { get; private set; }
        public void AddComponent(GameComponent component)
        {
            if (components == null)
            {
                components = new List<GameComponent>();
            }
            component.owner = this;
            components.Add(component);
        }

        public GameComponent GetComponent(GameConsts.COMPONENT_TYPE type)
        {
            if (components == null)
            {
                return null;
            }
            return components.Where(x => x.type == type).FirstOrDefault();
        }

        #endregion Components

    }
}
