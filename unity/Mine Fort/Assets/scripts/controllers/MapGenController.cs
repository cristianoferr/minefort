using Rimworld;
using Rimworld.logic.MapGen;
using Rimworld.model.entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenController : MonoBehaviour {

    MapGenerator mapGen;
	// Use this for initialization
	void Start () {
        mapGen = new MapGenerator(World.Current);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void regenerateMap()
    {
        int seed = Utils.Random(0, 10000);
        mapGen.RegenerateMap(seed,  World.Current.biome);
    }
}
