using System;
using Rimworld.logic;
using Rimworld.model.entities;
using Rimworld.model.entities.map;
using Rimworld.model.io;

namespace Rimworld
{
    public class DataHolder
    {
        private World world;

        //public Biomes biomes;
        public Biome biome { get; set; }

        public DataHolder(World world)
        {
            this.world = world;
            templateInitializer = new TemplateInitializer(this);
            biome = new Biome();
            
        }

        public TemplateInitializer templateInitializer { get; private set; }

        public model.io.Templates templates { get; set; }

        internal void Start()
        {
            DataLoader.LoadDataFor(world);
        }
    }
}
