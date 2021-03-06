﻿using System.Collections.Generic;

namespace MineFort.model
{
    public class GameConsts
    {
        public enum Facing
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }

        public enum ENTERABILITY { Yes, Never, Soon }
  
        public enum BuildMode
        {
            FLOOR,
            FURNITURE,
            DECONSTRUCT
        }
        public enum DATA_TYPE
        {
            UNDEFINED = 0, INT = 1, FLOAT = 2, STRING = 3
        }
        public enum JOBS
        {
            UNDEFINED = 0, MINER = 1, HAULER = 2, FARMER = 3, COOKER = 4
        }

        public enum COMPONENT_TYPE
        {
            BRAIN = 1, STOMACH = 2, LEG = 3, ARM = 4,
            TRAIT_MANAGER = 5,
            BODY = 6, HEART = 7
        }

        public enum TRAITS
        {
            SHOOTING, MELEE, CRAFTING, SMITH,
            SOCIAL
        }

        public static IList<TRAITS> allTraits = new List<TRAITS>();
        static GameConsts()
        {
            allTraits.Add(TRAITS.CRAFTING);
            allTraits.Add(TRAITS.MELEE);
            allTraits.Add(TRAITS.SHOOTING);
            allTraits.Add(TRAITS.SMITH);
            allTraits.Add(TRAITS.SOCIAL);

        }


        public const int HUMAN_TRAITS_POINTS = 20;

        public const int MAX_TRAIT_VALUE = 10;

        public const string TEMPL_HUMANOID = "HUMANOID";
        public const string TEMPL_HUMANOID_BODYPARTS = "Hunaoid Bodyparts";

        public const string TAG_HUMANOID = "humanoid";
        public const string TAG_BODYPARTS = "bodyparts";
        public const string TAG_ORGANIC = "organic";
        public const string TAG_PHYSICAL = "physical";
        public const string TAG_BRAIN = "brain";
        public const string TAG_ORGAN = "organ";
        public const string TAG_HEART = "heart";
        public const string TAG_TRAITMANAGER = "traitManager";
        public const string TAG_STARTING_TOWN = "startingTown";

        public const string VAL_POINTS_TO_DISTRIBUTE = "POINTS_TO_DISTRIBUTE";


        public const string VAL_WORLD_WIDTH = "world_width";

        public const string VAL_WORLD_HEIGHT = "world_height";

        public const int CHUNK_QTD = 4;
        public static int CHUNK_SIZE = 32; //tamanho que cada chunk terá no eixo x,y
        public static int WORLD_WIDTH = GameConsts.CHUNK_SIZE * CHUNK_QTD;
        public static int WORLD_HEIGHT = CHUNK_SIZE * 4;
        //public static int WORLD_DEPTH= 1;//TODO: permitir multiplos niveis

        //usado no csv
        public const string DATA_DICT_BIOME = "BIOMES";
        internal const string DATA_DICT_TILEDATA = "TILEDATA";
        public const string DATA_DICT_AUTOMATO= "AUTOMATO";

        public const string TAG_TILE_WATER="water";
        public const string TAG_TILE_LAND = "land";
        public const string TAG_TILE_GRASS = "grass";
        public const string TAG_TILE_TEST = "test";
        public const string TAG_EMPTY = "empty";
        

        public const float CAM_MAX_DISTANCE = 50f;
        internal const float CHAR_TILE_HEIGHT = 1f;
    }
}
