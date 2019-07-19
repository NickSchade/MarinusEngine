using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LocationsMaker
{
    public static Dictionary<Pos, ILocation> MakeLocations(IGame game, GameType gameType, float percentSea, float percentRiver, float numCats = 3f)
    {
        if (gameType == GameType.OrganicGrowth)
        {
            return MakeLocationsForOrganicGrowth(game, percentSea, percentRiver);
        }
        else if (gameType == GameType.MapGenPaintedRegions)
        {
            return MakeLocationsForMapGenPaintedRegions(game.map, percentSea, percentRiver);
        }
        else if (gameType == GameType.MapGenBiomeColors)
        {
            return MakeLocationsForMapGenBiomeColors(game.map, percentSea, percentRiver);
        }
        else if (gameType == GameType.ManualBiome)
        {
            return MakeLocationsForBiomeFromManual(game.map, percentSea, percentRiver, numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromNoise)
        {
            return MakeLocationsForExodusBiomeFromNoise((ExodusGame)game, percentSea, percentRiver, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromClusters)
        {
            return MakeLocationsForExodusBiomeFromClustering((ExodusGame)game, percentSea, percentRiver, (int)numCats);
        }
        else if (gameType == GameType.MapGenPaintedRegions)
        {
            return MakeLocationsForMapGenPaintedRegions(game.map, percentSea, percentRiver);
        }
        else if (gameType == GameType.ExodusGame)
        {
            return MakeLocationsForExodus((ExodusGame)game, percentSea, percentRiver, (int)numCats);
        }
        else
        {
            throw new System.Exception("GameType not implemented in LocationMaker"); 
        }
    }
    public static Dictionary<Pos, ILocation> MakeLocationsForBiomeFromManual(IMap map, float percentSea, float percentRiver, float numCats)
    {
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();
        
        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
        mapGen.GenerateMap();

        Color[] colorList = Colors.ColorListProcedural(2000);
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                locations[p] = new LocationManualBiome(p, mapGen.GetLocationQualities(x, y), colorList, numCats);
            }
        }

        return locations;
    }
    public static Dictionary<Pos, ILocation> MakeLocationsForExodusBiomeFromNoise(ExodusGame game, float percentSea, float percentRiver, int numCats)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
        mapGen.GenerateMap();

        float[][,] biomes = new float[numCats][,];

        for (int iCat = 0; iCat < numCats; iCat++)
        {
            ElevationBuilder elevationBuilder = new ElevationBuilder(MapUtil.nFromDims(map.xDim, map.yDim));
            elevationBuilder.SetElevationWithMidpointDisplacement(1);
            biomes[iCat] = elevationBuilder.Elevation;
            for (int x = 0; x < map.xDim; x++)
            {
                for (int y = 0; y < map.yDim; y++)
                {
                    biomes[iCat][x,y] = mapGen.Elevation[x, y] > mapGen.seaLevel && biomes[iCat][x, y] > ExodusSettings.biomeCutoffDiscrete ? biomes[iCat][x,y] : 0f;
                }
            }

        }



        Color[] colorList = Colors.ColorListProcedural(2000);
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                float[] thisBiomes = new float[numCats];
                for (int iCat = 0; iCat < numCats; iCat++)
                {
                    thisBiomes[iCat] = biomes[iCat][x, y];
                }
                Dictionary<string, float> qualities = mapGen.GetLocationQualities(x, y);
                locations[p] = new ExodusLocation(p, thisBiomes, game, qualities);
            }
        }

        return locations;
    }
    public static Dictionary<Pos, ILocation> MakeLocationsForExodusBiomeFromClustering(ExodusGame game, float percentSea, float percentRiver, int numCats, float cutoff = 0.9f)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
        mapGen.GenerateMap();

        Clustering km = new Clustering();
        int[,] biomeClusters = km.ClusterMap(mapGen, numCats);

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                float[] thisBiomes = new float[numCats];
                thisBiomes[biomeClusters[x, y]] = 1f;
                Dictionary<string, float> qualities = mapGen.GetLocationQualities(x, y);
                locations[p] = new ExodusLocation(p, thisBiomes, game, qualities);
            }
        }

        return locations;
    }
    public static Dictionary<Pos, ILocation> MakeLocationsForExodus(ExodusGame game, float percentSea, float percentRiver, int numCats, float cutoff = 0.9f)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
        mapGen.GenerateMap();

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                Dictionary<string, float> qualities = mapGen.GetLocationQualities(x, y);
                locations[p] = new ExodusLocation(p, new float[numCats], game, qualities);
            }
        }

        return locations;
    }



    public static Dictionary<Pos, ILocation> MakeLocationsForMapGenPaintedRegions(IMap map, float percentSea, float percentRiver)
    {
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
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
    public static Dictionary<Pos, ILocation> MakeLocationsForMapGenBiomeColors(IMap map, float percentSea, float percentRiver)
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
    static Color ColorBiome(MapGen mapGen, int x, int y)
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
    public static Dictionary<Pos, ILocation> MakeLocationsForOrganicGrowth(IGame game, float percentSea, float percentRiver, float pow = 2f)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        ElevationBuilder elevationBuilder = new ElevationBuilder(MapUtil.nFromDims(map.xDim, map.yDim));
        elevationBuilder.SetElevationWithMidpointDisplacement(1);
        //elevationBuilder.TrimToDimensions(xDim, yDim);

        float[,] resources = elevationBuilder.Elevation;
        MapUtil.TransformMap(ref resources, MapUtil.dExponentiate, pow);
        MapGen mapGen = new MapGen(map.xDim, map.yDim, _percentSea: percentSea, _percentRiver: percentRiver);
        mapGen.GenerateMap();
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                Pos p = map.pathMap[new Loc(x, y).key()];
                bool isLand = mapGen.Elevation[x, y] < mapGen.seaLevel ? false : true;
                float startingValue = resources[x, y];
                locations[p] = new LocationOrganicGrowth(game, isLand, startingValue);
            }
        }

        return locations;
    }
}
