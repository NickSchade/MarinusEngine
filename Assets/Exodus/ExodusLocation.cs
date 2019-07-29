using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExodusLocation : ILocation
{
    public Pos _p;
    public float[] _biomes;
    public Dictionary<string, float> _qualities;
    ExodusGame _game;

    public ExodusLocation(Pos p, float[] biomes, ExodusGame game, Dictionary<string, float> qualities)
    {
        _p = p;
        _biomes = biomes;
        _game = game;
        _qualities = qualities;
        mDebug.Log("MakingExodusLocation", false);
    }
    public int GetMaxBiome()
    {
        float maxBiomeVal = float.NegativeInfinity;
        int maxBiomeIndex = 0;
        for (int i = 0; i < _biomes.Length; i++)
        {
            float biome = _biomes[i];
            if (biome > maxBiomeVal)
            {
                maxBiomeVal = biome;
                maxBiomeIndex = i;
            }
        }
        return maxBiomeIndex;
    }

    public void Click()
    {
        Describe(true);
        
    }
    
    public Color GetColor()
    {
        return _game._colorLocation.GetColor(this);
    }
    

    public void TakeTurn()
    {
        // doesn't do anything yet.....
    }

    public void Describe(bool log = true)
    {
        // this fuction logs details about this location
        if (log)
        {
            Debug.Log("This is where the description goes");
            Debug.Log("Biomes are " + string.Join(",", _biomes.Select(x=>x.ToString()).ToArray()));
            Debug.Log("Color is " + GetColor().ToString());
        }
    }
}

