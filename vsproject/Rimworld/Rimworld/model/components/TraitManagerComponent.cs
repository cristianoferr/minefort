using Rimworld.model.components.brain;
using System.Collections.Generic;

namespace Rimworld.model.components
{
    public class TraitManagerComponent : GameComponent
    {
        public TraitManagerComponent()
            : base(GameConsts.COMPONENT_TYPE.TRAIT_MANAGER)
        {
            traits = new List<Trait>();

        }

        public override void Initialize()
        {
            InitTraits(GetValueAsInt(GameConsts.VAL_POINTS_TO_DISTRIBUTE));
        }

        private void InitTraits(int pointsToDistribute)
        {
            foreach (GameConsts.TRAITS trait in GameConsts.allTraits)
            {
                traits.Add(new Trait(trait, 0));
            }
            while (pointsToDistribute > 0)
            {
                Trait trait = traits[Utils.Random(0, traits.Count)];
                if (trait.value < GameConsts.MAX_TRAIT_VALUE)
                {
                    pointsToDistribute--;
                    trait.value++;
                }
            }
        }

        public IList<Trait> traits { get; private set; }

    }
}
