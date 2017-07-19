using System;
using MineFort.logic;
using MineFort.model.entities;
using MineFort.model.entities.map;
using MineFort.model.io;
using MineFort.io;

namespace MineFort
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
