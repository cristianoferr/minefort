using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rimworld.model.components.brain
{
    public class Trait
    {
        private GameConsts.TRAITS trait;
        public int value { get; set; }

        public Trait(GameConsts.TRAITS trait, int value)
        {
            this.trait = trait;
            this.value = value;
        }
    }
}
