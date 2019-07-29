using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType { ExodusGame, MapGenPaintedRegions, MapGenBiomeColors, OrganicGrowth, ManualBiome, ExodusBiomeFromNoise, ExodusBiomeFromClusters }

public enum TileShape { SQUARE, HEX };

public static class mDebug
{
    public static void Log(string s, bool write = true)
    {
        if (write)
        {
            Debug.Log(s);
        }
    }
}
