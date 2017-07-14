using Rimworld.model.entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rimworld.controllers
{
    public class TileSpriteController : MonoBehaviour
    {

        Dictionary<Tile, GameObject> tileGameObjectMap;

        World world
        {
            get { return World.current; }
        }

        // Use this for initialization
        void Start()
        {
            // Instantiate our dictionary that tracks which GameObject is rendering which Tile data.
            tileGameObjectMap = new Dictionary<Tile, GameObject>();

            // Create a GameObject for each of our tiles, so they show visually. (and redunt reduntantly)
            for (int x = 0; x < world.mapData.width; x++)
            {
                for (int y = 0; y < world.mapData.height; y++)
                {
                    // Get the tile data
                    Tile tile_data = world.mapData.GetTileAt(x, y);

                    // This creates a new GameObject and adds it to our scene.
                    GameObject tile_go = new GameObject();

                    // Add our tile/GO pair to the dictionary.
                    tileGameObjectMap.Add(tile_data, tile_go);

                    tile_go.name = "Tile_" + x + "_" + y;
                    tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                    tile_go.transform.SetParent(this.transform, true);

                    // Add a Sprite Renderer
                    // Add a default sprite for empty tiles.
                    SpriteRenderer sr = tile_go.AddComponent<SpriteRenderer>();
                    sr.sprite = SpriteManager.current.GetSprite("Tile", "Empty");
                    sr.sortingLayerName = "Tiles";

                    OnTileChanged(tile_data);
                }
            }

            // Register our callback so that our GameObject gets updated whenever
            // the tile's type changes.
            world.RegisterTileChanged(OnTileChanged);
        }

        // THIS IS AN EXAMPLE -- NOT CURRENTLY USED (and probably out of date)
        void DestroyAllTileGameObjects()
        {
            // This function might get called when we are changing floors/levels.
            // We need to destroy all visual **GameObjects** -- but not the actual tile data!

            while (tileGameObjectMap.Count > 0)
            {
                Tile tile_data = tileGameObjectMap.Keys.First();
                GameObject tile_go = tileGameObjectMap[tile_data];

                // Remove the pair from the map
                tileGameObjectMap.Remove(tile_data);

                // Unregister the callback!
                tile_data.UnregisterTileTypeChangedCallback(OnTileChanged);

                // Destroy the visual GameObject
                Destroy(tile_go);
            }

            // Presumably, after this function gets called, we'd be calling another
            // function to build all the GameObjects for the tiles on the new floor/level
        }

        // This function should be called automatically whenever a tile's data gets changed.
        void OnTileChanged(Tile tile_data)
        {

            if (tileGameObjectMap.ContainsKey(tile_data) == false)
            {
                Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
                return;
            }

            GameObject tile_go = tileGameObjectMap[tile_data];

            if (tile_go == null)
            {
                Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe forget to unregister a callback?");
                return;
            }

            Utils.LogError("Verificar a saida do tile.toString(): " + tile_data.Type.ToString());
            tile_go.GetComponent<SpriteRenderer>().sprite = SpriteManager.current.GetSprite("Tile", tile_data.Type.ToString());
            /*if (tile_data.Type == GameConsts.TileType.Floor)
            {
                tile_go.GetComponent<SpriteRenderer>().sprite = SpriteManager.current.GetSprite("Tile", "Floor");
            }
            else if (tile_data.Type == GameConsts.TileType.Empty)
            {
                tile_go.GetComponent<SpriteRenderer>().sprite = SpriteManager.current.GetSprite("Tile", "Empty");
            }
            else
            {
                Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
            }*/


        }



    }
}
