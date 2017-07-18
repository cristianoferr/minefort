using System;
using Rimworld.logic;
using Rimworld.model.entities;
using Rimworld.model.entities.map;
using Rimworld.model.io;
using Rimworld.io;

namespace Rimworld
{
    public class DataHolder
    {
        private World world;

        //public Biomes biomes;

        public DataHolder(World world)
        {
            this.world = world;
            templateInitializer = new TemplateInitializer(this);
            
        }

        public TemplateInitializer templateInitializer { get; private set; }

        public model.io.Templates templates { get; set; }

        internal void Start()
        {
            DataLoader.LoadDataFor(world);
        }
    }
}
