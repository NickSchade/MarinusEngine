using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarinusMap : IMap
{
    public IGame game;
    public Dictionary<string, Pos> pathMap { get; set; }
    public Dictionary<Pos, ILocation> lands { get; set; }
    public Dictionary<Pos, IStructure> settlements { get; set; }
    public int xDim { get; set; }
    public int yDim { get; set; }
    public bool wrapEastWest { get; set; }
    public bool wrapNorthSouth { get; set; }
    public float percentSea { get; set; }
    public float percentRiver { get; set; }


    public GameType gameType;
    public MarinusMap(IGame _game, GameType _gameType, int _xDim, int _yDim, bool _wrapEastWest, bool _wrapNorthSouth, float _percentSea, float _percentRiver)
    {
        gameType = _gameType;
        _game.map = this;
        InitializeVars(_game, _xDim, _yDim, _wrapEastWest, _wrapNorthSouth, _percentSea, _percentRiver);
        Generate2DGrid(game.tileShape);
        MakeLands();
    }
    protected virtual void MakeLands()
    {
        lands = LocationsMaker.MakeLocations(game, gameType, percentSea, percentRiver, ExodusSettings.numberOfBiomes);
    }
    protected void InitializeVars(IGame _game, int _xDim, int _yDim, bool _wrapEastWest, bool _wrapNorthSouth, float _percentSea, float _percentRiver)
    {
        Debug.Log("Initializing vars in map base");
        game = _game;
        xDim = _xDim;
        yDim = _yDim;
        wrapEastWest = _wrapEastWest;
        wrapNorthSouth = _wrapNorthSouth;
        percentSea = _percentSea;
        percentRiver = _percentRiver;
    }

    protected void Generate2DGrid(TileShape tileShape)
    {
        mDebug.Log("Generating map.pathMap...", false);
        pathMap = new Dictionary<string, Pos>();

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                Loc l = new Loc(x, y);
                Pos p = new Pos(l, game);
                mDebug.Log("New Pos is called" + p.gridLoc.key(), false);
                pathMap[p.gridLoc.key()] = p;
            }
        }
        foreach (string k in pathMap.Keys)
        {
            Pos p = pathMap[k];
            p.SetNeighbors(this, tileShape);
        }
        mDebug.Log("...Finished map.pathMap", false);
    }
}

