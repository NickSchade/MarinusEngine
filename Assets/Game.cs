using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IGame
{
    TileShape tileShape { get; set; }
    Map map { get; set; }
    int tick { get; set; }

    bool HandleClick(Pos pos, bool leftClick, bool rightClick);
    void TakeTurn();
    
}

public class Game: IGame
{
    public TileShape tileShape { get; set; }
    public Map map { get; set; }
    public GameManager gameManager;
    public int tick { get; set; }


    int dim = 100;
    bool wrapEastWest = false;
    bool wrapNorthSouth = false;
    float percentSea = 0.75f;
    float percentRiver = 0.20f;

    public Color[] regionColors;
    public Game(GameManager _gameManager)
    {
        Initialize(_gameManager);
        regionColors = Colors.ColorListProcedural(200);
    }
    public void Initialize(GameManager _gameManager)
    {
        int xDim = dim;
        int yDim = dim;
        gameManager = _gameManager;
        tick = 0;
        map = new Map(this, xDim, yDim, wrapEastWest, wrapNorthSouth);
        map.lands = MakeLocations(map, gameManager.gameType);
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

    public Dictionary<Pos, ILocation> MakeLocations(Map map, GameType gameType)
    {
        if (gameType == GameType.HomelandsOrganic)
        {
            return MakeLocationsForHomelandsOrganic(map);
        }
        else if (gameType == GameType.MapGenPaintedRegions)
        {
            return MakeLocationsForMapGenPaintedRegions(map);
        }
        else if (gameType == GameType.MapGenBiomeColors)
        {
            return MakeLocationsForMapGenBiomeColors(map);
        }
        else
        {
            return MakeLocationsForMapGenPaintedRegions(map);
        }
    }
    public Dictionary<Pos, ILocation> MakeLocationsForMapGenPaintedRegions(Map map)
    {
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea:percentSea, _percentRiver:percentRiver);
        mapGen.GenerateMap();
        mapGen.PaintRegions();
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                locations[p] = new LocationMapGenPainted(mapGen.GetLocationQualities(x, y));
            }
        }

        return locations;
    }
    public Dictionary<Pos, ILocation> MakeLocationsForMapGenBiomeColors(Map map)
    {
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
        mapGen.GenerateMap();

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                Color c = ColorBiome(mapGen, x, y);
                locations[p] = new LocationMapGenBiomeColor(c);
            }
        }

        return locations;
    }
    Color ColorBiome(MapGen mapGen, int x, int y)
    {
        float r = mapGen.WaterFlux[x, y];
        float t = mapGen.Temperature[x, y];
        float e = mapGen.Elevation[x, y];
        Color orange = MapColor.GetColorLerp(0.5f, Color.red, Color.yellow);
        Color cr = MapColor.GetColorLerp(r, orange, Color.green);
        Color ct = MapColor.GetColorLerp(t, Color.black, Color.white);
        Color c = MapColor.GetColorLerp(0.5f, cr, ct);
        c = e < mapGen.seaLevel ? Color.blue : c;
        c = (mapGen.Temperature[x, y] < mapGen.iceLevel) ? Color.white : c;
        c = (mapGen.WaterFlux[x, y] > mapGen.riverLevel) && e > mapGen.seaLevel ? MapColor.GetColorLerp(0.5f, Color.blue, Color.cyan) : c;
        return c;

    }
    public Dictionary<Pos, ILocation> MakeLocationsForHomelandsOrganic(Map map, float pow = 2f)
    {
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        ElevationBuilder elevationBuilder = new ElevationBuilder(MapUtil.nFromDims(map.xDim, map.yDim));
        elevationBuilder.SetElevationWithMidpointDisplacement(1);
        //elevationBuilder.TrimToDimensions(xDim, yDim);

        float[,] resources = elevationBuilder.Elevation;
        MapUtil.TransformMap(ref resources, MapUtil.dExponentiate, pow);
        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver );
        mapGen.GenerateMap();
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                bool isLand = mapGen.Elevation[x, y] < mapGen.seaLevel ? false : true;
                float startingValue = resources[x, y];
                locations[p] = new LocationHomelandsOrganic(this, isLand, startingValue);
            }
        }

        return locations;
    }
}



public enum GameType { MapGenPaintedRegions, MapGenBiomeColors, HomelandsOrganic}


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
    void TakeTurn();
    void Click();
}

public class LocationHomelandsOrganic : ILocation
{
    float resourceValue;
    bool isLand;
    float maxResource = 1f;
    float resourcePerTurn = 0.0001f;
    float resourcePow = 0.99f;//0.9999f;//0.99999f;
    bool isVisible = true;
    Game game;
    public LocationHomelandsOrganic(Game _game, bool _isLand, float _startingResourceValue)
    {
        game = _game;
        isLand = _isLand;
        resourceValue = _startingResourceValue;
    }
    public void TakeTurn()
    {
        if (resourceValue < maxResource)
        {
            //resourceValue += resourcePerTurn;
            resourceValue = Mathf.Pow(resourceValue, resourcePow);
        }
        else if (resourceValue > maxResource)
        {
            resourceValue = maxResource;
        }
    }
    public Color GetColor()
    {
        if (isVisible)
        {
            if (isLand)
            {
                return Colors.OceanBlue;
            }
            else
            {
                mDebug.Log("resourceValue = " + resourceValue, false);
                //float x = Mathf.Round(resourceValue * 10) / 10f;
                float x = resourceValue;
                return Colors.GetColorLerp(x / maxResource, Color.white, Color.black);
            }
        }
        else
        {
            return Color.black;
        }
    }

    public void Click()
    {
        isVisible = !isVisible;
    }
}

public abstract class LocationMapGen : ILocation
{
    public abstract Color GetColor();
    public void TakeTurn()
    {

    }
    public void Click()
    {

    }
}

public class LocationMapGenPainted : LocationMapGen, ILocation
{
    public Dictionary<string, float> qualities;
    public LocationMapGenPainted(Dictionary<string, float> _qualities)
    {
        qualities = _qualities; 
    }
    public override Color GetColor()
    {
        string Category = "Regions";
        float value = qualities[Category];
        Color c = Colors.GetColorLerp(value, Color.blue, Color.green);
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

}

public class LocationMapGenBiomeColor : LocationMapGen, ILocation
{
    Color color;
    public LocationMapGenBiomeColor(Color c) 
    {
        color = c;
    }
    public override Color GetColor()
    {
        return color;

    }

}


