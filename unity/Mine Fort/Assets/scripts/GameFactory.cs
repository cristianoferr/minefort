﻿
namespace MineFort
{
    public class GameFactory
    {
        public GameFactory(DataHolder holder)
        {
            this.holder = holder;
            this.world = World.Current;
        }
       /* public GameCharacter SpawnPawn(int pointsToSpend)
        {
            return world.AddEntity(new GameCharacter()) as GameCharacter;
        }*/
/*
        public model.entities.physical.GEStockPile SpawnStockPile(int x, int y, int width, int height)
        {
            GEStockPile pile = new GEStockPile();
            pile.position.x = x;
            pile.position.y = y;
            pile.dimension.width = width;
            pile.dimension.height = height;
            return world.AddEntity(pile) as GEStockPile;

        }

        public GETownCenter SpawnTownCenter(string name, int x, int y)
        {
            GETownCenter pile = new GETownCenter();
            pile.name = name;
            pile.position.x = x;
            pile.position.y = y;
            world.AddEntity(pile);
            return pile;
        }*/

        public DataHolder holder { get; set; }

        public World world { get; set; }
    }
}
