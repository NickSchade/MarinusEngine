using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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

public interface IGame
{
    TileShape tileShape { get; set; }
    IMap map { get; set; }
    int tick { get; set; }

    List<int> activeBiomes { get; set; }
    Color[] biomeColors { get; set; }

    bool HandleClick(Pos pos, bool leftClick, bool rightClick);
    void TakeTurn();
}

public class Game: IGame
{
    public TileShape tileShape { get; set; }
    public IMap map { get; set; }
    public GameManager gameManager;
    public int tick { get; set; }

    public List<int> activeBiomes { get; set; }
    public Color[] biomeColors { get; set; }

    protected int dim;
    protected bool wrapEastWest = false;
    protected bool wrapNorthSouth = false;
    protected float percentSea = 0.5f;
    protected float percentRiver = 0.20f;
    
    public Game(GameManager _gameManager)
    {
        Initialize(_gameManager);
    }
    public void Initialize(GameManager _gameManager)
    {
        gameManager = _gameManager;
        dim = gameManager.dim;
        int xDim = dim;
        int yDim = dim;
        tick = 0;
        //map = new Map(this, gameManager.gameType, xDim, yDim, wrapEastWest, wrapNorthSouth, percentSea, percentRiver);
    }

    public bool HandleClick(Pos pos, bool leftClick, bool rightClick)
    {
        map.lands[pos].Click();
        return true;
    }

    public void TakeTurn()
    {
        tick++;
        foreach (ILocation loc in map.lands.Values)
        {
            loc.TakeTurn();
        }
    }
    
}



public enum GameType { ExodusGame, MapGenPaintedRegions, MapGenBiomeColors, OrganicGrowth, ManualBiome, ExodusBiomeFromNoise, ExodusBiomeFromClusters}



public interface IMap
{
    Dictionary<string, Pos> pathMap { get; set; }
    Dictionary<Pos, ILocation> lands { get; set; }
    int xDim { get; set; }
    int yDim { get; set; }
    bool wrapEastWest { get; set; }
    bool wrapNorthSouth { get; set; }
}

public class Map : IMap
{
    public IGame game;
    public Dictionary<string, Pos> pathMap { get; set; }
    public Dictionary<Pos, ILocation> lands { get; set; }
    public int xDim { get; set; }
    public int yDim { get; set; }
    public bool wrapEastWest { get; set; }
    public bool wrapNorthSouth { get; set; }
    public float percentSea { get; set; }
    public float percentRiver { get; set; }
    public GameType gameType;
    public Map(IGame _game, GameType _gameType, int _xDim, int _yDim, bool _wrapEastWest, bool _wrapNorthSouth, float _percentSea, float _percentRiver)
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
                mDebug.Log("New Pos is called" + p.gridLoc.key(),false);
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



public enum TileShape { SQUARE, HEX};




