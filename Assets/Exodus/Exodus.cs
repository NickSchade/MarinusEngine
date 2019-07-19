using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExodusSettings
{
    public static float biomeCutoffDiscrete = 0.7f;
    public static float biomeCutoffContinuous = 0.5f;

    public static int numberOfBiomes = 4;
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
        activeBiomes = new List<int> { 0, 1 ,2 ,3 };
        biomeColors = Colors.ColorListProcedural(ExodusSettings.numberOfBiomes);
    }
    public void rotateBiomeDiscreteSingle()
    {
        int biome = activeBiomes[0];
        biome = biome == biomeColors.Length - 1 ? 0 : biome + 1;
        activeBiomes = new List<int> { biome };
    }
}
