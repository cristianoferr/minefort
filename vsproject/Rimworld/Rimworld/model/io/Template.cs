using Rimworld.model.components;
using Rimworld.model.entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rimworld.model.io
{
    [DataContract]
    public class Template : Property
    {
        public Template(string name)
            : base(name)
        {
            subComponents = new List<Property>();
            values = new List<Property>();
        }

        [DataMember]
        public IList<Property> subComponents;
        [DataMember]
        public IList<Property> values;

        internal Property AddComponent(string name = "noname")
        {
            Property prop = new Property(name);
            subComponents.Add(prop);
            return prop;
        }

        public System.Type entityToSpawn { get; set; }



        public Property AddComponentWithTags(string p1, string p2 = "", string p3 = "", string p4 = "", string p5 = "", string p6 = "", string p7 = "")
        {
            Property prop = new Property(p1 + p2 + p3 + p4 + p5 + p6 + p7);
            subComponents.Add(prop);
            prop.AddTag(p1);
            prop.AddTag(p2);
            prop.AddTag(p3);
            prop.AddTag(p4);
            prop.AddTag(p5);
            prop.AddTag(p6);
            prop.AddTag(p7);
            return prop;
        }

        public GameEntity Spawn(World world, Position pos = null)
        {
            Utils.Log("Spawning template: " + tagsAsText);
            GameEntity result = Activator.CreateInstance(entityToSpawn) as GameEntity;
            PhysicalEntity physEnt = result as PhysicalEntity;
            if (physEnt != null)
            {

                world.AddEntity(physEnt);
                physEnt.PlaceNear(pos);
            }

            ApplyValues(result);

            foreach (Property subComp in subComponents)
            {
                int qtdToSpawn = subComp.CalcQtyToSpawn();
                for (int i = 0; i < qtdToSpawn; i++)
                {
                    SpawnSubComponent(world, pos, result, subComp);
                }

            }
            result.Initialize();
            return result;
        }

        private void SpawnSubComponent(World world, Position pos, GameEntity result, Property subComp)
        {
            Template subTempl = owner.GetTemplateWithTag(subComp);
            if (subTempl == null)
            {
                throw new Exception("Nenhum template com a tag '" + subComp.tagsAsText + "' encontrado!");
            }
            GameEntity subEnt = subTempl.Spawn(world, pos);
            GameComponent gameComp = subEnt as GameComponent;
            if (gameComp != null)
            {
                result.AddComponent(gameComp);
            }
        }

        private void ApplyValues(GameEntity result)
        {
            foreach (Property prop in values)
            {
                GameValue value = new GameValue();
                value.dataType = prop.value.dataType;
                value.minValue = prop.value.minValue;
                value.maxValue = prop.value.maxValue;
                value.currValue = prop.value.currValue;
                if (prop.randomValue)
                {
                    if (value.dataType == GameConsts.DATA_TYPE.INT || value.dataType == GameConsts.DATA_TYPE.FLOAT)
                    {
                        value.currValue = Utils.Random(value.minValue, value.maxValue);
                    }
                }

                result.AddValue(prop.name, value);
            }
        }

        internal void AddIntValue(string name, int min, int max)
        {
            Property prop = new Property(name);
            GameValue value = new GameValue();
            value.dataType = GameConsts.DATA_TYPE.INT;
            value.minValue = min;
            value.maxValue = max;
            prop.value = value;
            values.Add(prop);
        }

        public Templates owner { get; set; }
    }
}
