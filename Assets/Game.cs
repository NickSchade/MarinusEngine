using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IGame
{
    TileShape tileShape { get; set; }
    Map map { get; set; }

    bool HandleClick(Pos pos, bool leftClick, bool rightClick);

    Dictionary<Pos, ILocation> MakeLocations(Map map);
    
}

public class Game: IGame
{
    public TileShape tileShape { get; set; }
    public Map map { get; set; }
    public GameManager gameManager;

    public Color[] regionColors;
    public Game(GameManager _gameManager)
    {
        Initialize(_gameManager);
        regionColors = Colors.ColorListProcedural(200);
    }
    public void Initialize(GameManager _gameManager)
    {
        gameManager = _gameManager;
        int dim = 50;
        int xDim = dim;
        int yDim = dim;
        bool wrapEastWest = false;
        bool wrapNorthSouth = false;
        map = new Map(this, xDim, yDim, wrapEastWest, wrapNorthSouth);
        map.lands = MakeLocations(map);
    }

    public bool HandleClick(Pos pos, bool leftClick, bool rightClick)
    {
        //mDebug.Log("In Game.HandleClick(), clicked on " + pos.getName());
        //mDebug.Log("It's neighbors are " + pos.listNeighbors());
        map.lands[pos].GetColor();
        return true;
    }

    public Dictionary<Pos, ILocation> MakeLocations(Map map)
    {
        int xDim = map.xDim;
        int yDim = map.yDim;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(xDim, yDim);
        mapGen.GenerateMap();
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                locations[p] = new LocationExodus(mapGen.GetLocationQualities(x, y));
            }
        }

        return locations;
    }
}

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

public class Map
{
    IGame game;
    public Dictionary<string, Pos> pathMap;
    public Dictionary<Pos, ILocation> lands;
    [System.NonSerialized] public int xDim, yDim;
    [System.NonSerialized] public bool wrapEastWest, wrapNorthSouth;
    public Map(IGame _game, int _xDim, int _yDim, bool _wrapEastWest, bool _wrapNorthSouth)
    {
        mDebug.Log("Cosntructing Map...");
        game = _game;
        xDim = _xDim;
        yDim = _yDim;
        wrapEastWest = _wrapEastWest;
        wrapNorthSouth = _wrapNorthSouth;

        Generate2DGrid(game.tileShape);
    }
    
    void Generate2DGrid(TileShape tileShape)
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


public interface ILocation
{
    Color GetColor();
}

public class LocationExodus : ILocation
{
    Dictionary<string, float> qualities;
    public LocationExodus(Dictionary<string,float> _qualities)
    {
        qualities = _qualities;
    }
    public Color GetColor()
    {
        string Category = "Regions";
        float value = qualities[Category];
        Color c = GetColorLerp(value, Color.blue, Color.green);
        if (Category == "Regions")
        {
            if (value == 0f)
            {
                c = Colors.DarkBlue;
            }
            else
            {
                c = Colors.staticColors[(int)value];
            }
        }
        mDebug.Log(Category + " value is " + value + " and color is " + c);
        return c;
        
    }
    static public Color GetColorLerp(float value, Color cLower, Color cUpper)
    {
        float r = Mathf.Lerp(cLower.r, cUpper.r, value);
        float g = Mathf.Lerp(cLower.g, cUpper.g, value);
        float b = Mathf.Lerp(cLower.b, cUpper.b, value);
        Color C = new Color(r, g, b);
        return C;
    }
}


