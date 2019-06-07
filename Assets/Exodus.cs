using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExodusSettings
{
    public static float biomeCutoffDiscrete = 0.7f;
    public static float biomeCutoffContinuous = 0.5f;

    public static int numberOfBiomes = 4;
}

public class ExodusLocation : ILocation
{
    Pos _p;
    float[] _biomes;
    Dictionary<string, float> _qualities;
    IGame _game;
    public ExodusLocation(Pos p, float[] biomes, IGame game, Dictionary<string,float> qualities)
    {
        _p = p;
        _biomes = biomes;
        _game = game;
        _qualities = qualities;
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
            c = GetColorContinuousSingleBiome(c);
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


public class ExodusMap : Map, IMap
{
    public ExodusMap(IGame _game, GameType _gameType, int _xDim, int _yDim, bool _wrapEastWest, bool _wrapNorthSouth, float _percentSea, float _percentRiver) : base(_game, _gameType, _xDim, _yDim, _wrapEastWest, _wrapNorthSouth, _percentSea, _percentRiver)
    {
        gameType = _gameType;
        _game.map = this;
        InitializeVars(_game, _xDim, _yDim, _wrapEastWest, _wrapNorthSouth, _percentSea, _percentRiver);
        Generate2DGrid(_game.tileShape);
        MakeLands();
    }
}


public class ExodusGame : Game, IGame
{
    public ExodusGame(GameManager gameManager) : base(gameManager)
    {
        map = new ExodusMap(this, gameManager.gameType, dim, dim, wrapEastWest, wrapNorthSouth, percentSea, percentRiver);
        activeBiomes = new List<int> { 0 };
        biomeColors = Colors.ColorListProcedural(ExodusSettings.numberOfBiomes);
    }
    public void rotateBiomeDiscreteSingle()
    {
        int biome = activeBiomes[0];
        biome = biome == biomeColors.Length - 1 ? 0 : biome + 1;
        activeBiomes = new List<int> { biome };
    }
}
