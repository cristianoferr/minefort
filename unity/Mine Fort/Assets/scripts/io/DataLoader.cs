using MineFort.model;
using MineFort.model.entities;
using MineFort.model.entities.map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MineFort.io
{
    public class DataLoader
    {
        static World world;
        internal static void LoadDataFor(World world)
        {
            DataLoader.world = world;
            
            LoadCSVDataFromDirectory(System.IO.Path.Combine(Application.streamingAssetsPath, "CSV"));
        }

        private static void LoadCSVDataFromDirectory(string filePath)
        {
            Utils.Log("LoadCSVDataFromDirectory: " + filePath);
            // First, we're going to see if we have any more sub-directories,
            // if so -- call LoadSpritesFromDirectory on that.

            string[] subDirs = Directory.GetDirectories(filePath);
            foreach (string sd in subDirs)
            {
                LoadCSVDataFromDirectory(sd);
            }

            string[] filesInDir = Directory.GetFiles(filePath);
            foreach (string fn in filesInDir)
            {

                string csvFile = new DirectoryInfo(filePath).Name;

                LoadCSV(csvFile, fn);
            }

        }

        private static void LoadCSV(string spriteCategory, string filePath)
        {
            if (!filePath.EndsWith(".csv"))
            {
                return;
            }
            string[] csvLines = System.IO.File.ReadAllLines(filePath);
            filePath = filePath.Replace("\\", "/").Replace(".csv", "");
            filePath = filePath.Substring(filePath.LastIndexOf("/")+1);
            int lineStart = 0;
            string dataDict = "undef";
            for (int i = 0; i < csvLines.Length; i++)
            {

                string[] lineData = csvLines[i].Split(';');
                if (i == 0) {
                    lineStart = int.Parse(lineData[0])-1;
                    dataDict = lineData[1];
                    
                }
                if (i == 1)
                {
                    ReadHeader(lineData, dataDict, filePath);
                }
                    if (i >= lineStart)
                {
                    ReadCSVLine(lineData, dataDict, filePath);
                }
            }

        }

        private static void ReadHeader(string[] lineData, string dataDict, string fileName)
        {
            if (dataDict == GameConsts.DATA_DICT_BIOME)
            {
                Biome biome = world.biomes.GetBiome(fileName);
                biome.maxHeight = int.Parse(lineData[2]);
                //biome.minHeight = float.Parse(lineData[3]);
                biome.scale = float.Parse(lineData[4]);
            }
        }

        private static void ReadCSVLine(string[] lineData,string dataDict, string fileName)
        {
            if (dataDict == GameConsts.DATA_DICT_BIOME)
            {
                Biome biome = world.biomes.GetBiome(fileName);
                biome.LoadFromCSV(lineData);
            }

            if (dataDict == GameConsts.DATA_DICT_TILEDATA)
            {
                world.biomes.LoadTileTypeFromCSV(lineData);
            }
        }
    }
}
