using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Rimworld;
using Rimworld.model.entities.physical;
using Rimworld.model.entities;
using Rimworld.model.io;
using Rimworld.model;
using Rimworld.model.components;
using Rimworld.model.components.brain;
using Rimworld.model.entities.map;
using Rimworld.model.entities.map.tiles;

public class InitialTest
{
    static World world = World.current;
    static DataHolder holder = world.holder;
    static GameFactory factory = new GameFactory(holder);


    [UnityTest]
    public IEnumerator TestaTileSet()
    {
        // TileSet tileSet = holder.tileSet;
        // Biomes biomes = holder.biomes;
        //Assert.IsNotNull(biomes);

        Biomes biomes = new Biomes();
        Biome biome = new Biome(biomes);
        //TileSet tileSet = biome.tileSet;

        biomes.AddBiome("Tropical",biome);
        Assert.IsTrue(biomes.GetBiome("Tropical") == biome);
        Biome biomeTest = biomes.GetBiome("Tropical");
        Assert.IsNotNull(biomeTest);
        Biome biomeTestNull = biomes.GetBiome("Tropical!!!");
        Assert.IsNull(biomeTestNull);
        Assert.IsTrue(biomeTest==biome);

        Tile tile = world.GetTileAt(0, 0);
        Assert.IsNotNull(tile);

       // TileData tileDataGround = new TileData();


        biome = world.biome;
        Assert.IsNotNull(biome);
      //  Assert.IsNotNull(biome.tileData);

       /* TileData td = biome.GetTileWithTag(GameConsts.TAG_TILE_TEST);
        Assert.IsTrue(td.name=="test");
        Assert.IsTrue(td.fileName == "ISO_Tile_Water_Block");
        Assert.IsTrue(td.MovementCost == 0.51f);
        Assert.IsTrue(td.MinHeight == 0);
        Assert.IsTrue(td.MaxHeight == 0.5f);*/

        yield return null;
    }

    [Test]
    public void TestaConversaoCoordenadaIso()
    {

        Vector3 pos = new Vector3(100, 50, 0);
        Vector3 posIso = Utils.TwoDToIso(pos.x, pos.y);
        Assert.IsNotNull(posIso);
        Vector3 posNormal = Utils.IsoTo2D(posIso.x, posIso.y);
        Assert.IsNotNull(posNormal);

        Assert.IsTrue(posNormal.x == pos.x, posNormal.x + "<>" + pos.x);
        Assert.IsTrue(posNormal.y == pos.y, posNormal.y + "<>" + pos.y);

    }

    [Test]
    public void TestSpawnStockPile()
    {
        /*GEStockPile stockPile = factory.SpawnStockPile(10, 20, 5, 8);
        Assert.IsNotNull(stockPile);
        Assert.IsTrue(world.ContainsEntity(stockPile));
        Assert.IsTrue(stockPile.position.x == 10);
        Assert.IsTrue(stockPile.position.y == 20);
        Assert.IsTrue(stockPile.dimension.width == 5);
        Assert.IsTrue(stockPile.dimension.height == 8);*/
    }

    [Test]
    public void TestTemplateInitializer()
    {

        Template templHuman = holder.templates.GetTemplate(GameConsts.TEMPL_HUMANOID);
        Assert.IsNotNull(templHuman);
        Assert.IsTrue(templHuman.name == GameConsts.TEMPL_HUMANOID);

        Template templHumanWithTag = holder.templates.GetTemplateWithTag(GameConsts.TAG_HUMANOID + " " + GameConsts.TAG_ORGANIC);
        Assert.IsNotNull(templHumanWithTag);
        Assert.IsTrue(templHumanWithTag == templHuman);

        Template templNullTag = holder.templates.GetTemplateWithTag(GameConsts.TAG_HUMANOID + " lixo " + GameConsts.TAG_ORGANIC);
        Assert.IsNull(templNullTag);

        Rimworld.model.entities.GameCharacter human = templHuman.Spawn(world, new Vector3(10, 10, 0)) as Rimworld.model.entities.GameCharacter;
        Assert.IsNotNull(human);

        TraitManagerComponent traits = human.GetComponent(GameConsts.COMPONENT_TYPE.TRAIT_MANAGER) as TraitManagerComponent;
        Assert.IsNotNull(traits);
        Assert.IsTrue(traits.owner == human);
        Assert.IsTrue(traits.traits.Count == GameConsts.allTraits.Count);
        int count = 0;
        foreach (Trait trait in traits.traits)
        {
            Assert.IsTrue(trait.value >= 0 && trait.value <= GameConsts.MAX_TRAIT_VALUE);
            count += trait.value;
        }
        Assert.IsTrue(count > 0);
    }

    [Test]
    public void TestWorldMap()
    {
        World world = World.current;
        Assert.IsNotNull(world);
        Assert.IsNotNull(world.mapData);
        Assert.IsNotNull(world.mapData.GetOutsideRoom(new Vector3(0, 0,0)));

        Tile tile = world.mapData.GetTileAt(10, 20);
        Assert.IsNotNull(tile);
        Assert.IsTrue(tile.position.x == 10);
        Assert.IsTrue(tile.position.y == 20);
        Assert.IsNotNull(tile.room);
        Assert.IsTrue(tile.room == tile.chunk.outsideRoom);

        Chunk chunk00 = world.mapData.GetChunkAt(0, 0);
        Chunk chunk10 = world.mapData.GetChunkAt(GameConsts.CHUNK_SIZE, 0);
        Assert.IsNotNull(chunk00);
        Assert.IsNotNull(chunk10);
        //testando se a posição coincide...
        Tile tile00 = chunk00.GetTileAt(GameConsts.CHUNK_SIZE, 0);
        Tile tile10 = chunk10.GetTileAt(0, 0);
        Tile tileChunk = world.mapData.GetTileAt(GameConsts.CHUNK_SIZE, 0);

        Assert.IsTrue(tile00 == tile10, tile00 + " <> " + tile10);
        Assert.IsTrue(tile00 == tileChunk, tile00 + " <> " + tileChunk);

        Chunk chunkM0 = world.mapData.GetChunkAt(GameConsts.WORLD_WIDTH - 1, 0);//retorna último chunk
        Chunk chunkN0 = world.mapData.GetChunkAt(-1, 0);//é para retornar para o ultimo chunk 
        Chunk chunkT0 = world.mapData.GetChunkAt(GameConsts.WORLD_WIDTH, 0);//retorna o 1o chunk

        Assert.IsTrue(chunkM0 == chunkN0, chunkM0 + " <> " + chunkN0);
        Assert.IsTrue(chunk00 == chunkT0, chunk00 + " <> " + chunkT0);

        Chunk chunkMinus = world.mapData.GetChunkAt(-100*GameConsts.WORLD_WIDTH , 0);
        Assert.IsNotNull(chunkMinus);
        Chunk chunkMax = world.mapData.GetChunkAt(100 * GameConsts.WORLD_WIDTH, 0);
        Assert.IsNotNull(chunkMax);

    }

    [Test]
    public void TestMapChunks()
    {

    }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        /*[UnityTest]
        public IEnumerator InitialTestWithEnumeratorPasses() {

            yield return null;
        }*/
    }
