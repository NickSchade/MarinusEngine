using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExodusLocation : ILocation
{
    Pos _p;
    float[] _biomes;
    Dictionary<string, float> _qualities;
    ExodusGame _game;
    public ExodusLocation(Pos p, float[] biomes, ExodusGame game, Dictionary<string, float> qualities)
    {
        _p = p;
        _biomes = biomes;
        _game = game;
        _qualities = qualities;
        mDebug.Log("MakingExodusLocation", false);
    }

    public void Click()
    {
        Describe(true);
    }

    public Color GetColor()
    {
        Color c = Color.magenta;
        if (_qualities["Land"] == 0f)
        {
            c = Color.black;
        }
        else
        {
            c = Color.white;
            //c = GetColorDiscreteSingleBiome(c);
            //c = GetColorContinuousSingleBiome(c);
            //c = GetColorDiscreteManyBiome(c);
            //c = GetColorContinuousManyBiome(c);
        }
        return c;
    }

    private Color GetColorDiscreteSingleBiome(Color currentColor)
    {
        int biomeIndex = _game.activeBiomes[0];
        if (_biomes[biomeIndex] > ExodusSettings.biomeCutoffDiscrete)
        {
            currentColor = _game.biomeColors[biomeIndex];
        }

        return currentColor;
    }

    private Color GetColorDiscreteManyBiome(Color currentColor)
    {
        for (int i = 0; i < _game.activeBiomes.Count; i++)
        {
            int biomeIndex = _game.activeBiomes[i];
            if (_biomes[biomeIndex] > ExodusSettings.biomeCutoffDiscrete)
            {
                currentColor = _game.biomeColors[biomeIndex];
                mDebug.Log(_p.getName() + "is" + currentColor);
            }
        }
        return currentColor;
    }

    private Color GetColorContinuousSingleBiome(Color currentColor)
    {
        int biomeIndex = _game.activeBiomes[0];
        if (_biomes[biomeIndex] > ExodusSettings.biomeCutoffContinuous)
        {
            Color pureColor = _game.biomeColors[biomeIndex];
            currentColor = MapColor.GetColorLerp(_biomes[biomeIndex], Color.white, pureColor);
        }

        return currentColor;
    }

    private Color GetColorContinuousManyBiome(Color currentColor)
    {
        for (int i = 0; i < _game.activeBiomes.Count; i++)
        {
            int biomeIndex = _game.activeBiomes[i];
            if (_biomes[biomeIndex] > ExodusSettings.biomeCutoffContinuous)
            {
                Color pureColor = _game.biomeColors[biomeIndex];
                currentColor = MapColor.GetColorLerp(_biomes[biomeIndex], Color.white, pureColor);
            }
        }

        return currentColor;
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
            Debug.Log("Color is " + GetColor().ToString());
        }
    }
}
