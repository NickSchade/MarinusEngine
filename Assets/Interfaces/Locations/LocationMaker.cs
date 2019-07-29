using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeInitialization { Noise, Clustering, Painting, BasicWaterTemp, AdvancedWaterTemp, AutomaticWaterTemp, AutomaticWaterTempLongLat };

public static class LocationsMaker
{
    public static Dictionary<Pos, ILocation> MakeLocations(IGame game, GameType gameType, MapGen mapGen, float numCats = 3f)
    {
        if (gameType == GameType.OrganicGrowth)
        {
            return MakeLocationsForOrganicGrowth(game, mapGen);
        }
        else if (gameType == GameType.MapGenBiomeColors)
        {
            return MakeLocationsForMapGenBiomeColors(game.map, mapGen);
        }
        else if (gameType == GameType.ExodusBiomeFromNoise)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.Noise, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromClusters)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.Clustering, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromPainted)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.Painting, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromBasicWaterTemp)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.BasicWaterTemp, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromAdvancedWaterTemp)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.AdvancedWaterTemp, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromAutomaticWaterTemp)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.AutomaticWaterTemp, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusBiomeFromAutomaticWaterTempLongLat)
        {
            return MakeLocationsForExodusFromMethod(BiomeInitialization.AutomaticWaterTempLongLat, (ExodusGame)game, mapGen, (int)numCats);
        }
        else if (gameType == GameType.ExodusGame)
        {
            return MakeLocationsForExodus((ExodusGame)game, mapGen, (int)numCats);
        }
        else
        {
            throw new System.Exception("GameType not implemented in LocationMaker");
        }
    }


    public static Dictionary<Pos, ILocation> MakeLocationsForExodusFromMethod(BiomeInitialization biomeInit, ExodusGame game, MapGen mapGen, int numCats)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        float[][,] biomes = ExodusBiomeInitializer.GetBiomesFromMethod(map, mapGen, numCats, biomeInit);

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
    public static Dictionary<Pos, ILocation> MakeLocationsForExodus(ExodusGame game, MapGen mapGen, int numCats, float cutoff = 0.9f)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

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

    public static Dictionary<Pos, ILocation> MakeLocationsForMapGenBiomeColors(IMap map, MapGen mapGen)
    {
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

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
    public static Dictionary<Pos, ILocation> MakeLocationsForOrganicGrowth(IGame game, MapGen mapGen, float pow = 2f)
    {
        IMap map = game.map;
        Dictionary<Pos, ILocation> locations = new Dictionary<Pos, ILocation>();

        ElevationBuilder elevationBuilder = new ElevationBuilder(MapUtil.nFromDims(map.xDim, map.yDim));
        elevationBuilder.SetElevationWithMidpointDisplacement(1);
        //elevationBuilder.TrimToDimensions(xDim, yDim);

        float[,] resources = elevationBuilder.Elevation;
        MapUtil.TransformMap(ref resources, MapUtil.dExponentiate, pow);
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
