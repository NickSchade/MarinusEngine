using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExodusBiomeInitializer 
{
    public static float[,][] GetBiomes(IMap map, MapGen mapGen, int numCats, BiomeInitialization biomeInit)
    {
        float[,][] biomes = new float[map.xDim, map.yDim][];
        switch (biomeInit)
        {
            case BiomeInitialization.Clustering:
                biomes = GetBiomesFromClustering2(map, mapGen, numCats);
                break;
            case BiomeInitialization.Noise:
                biomes = GetBiomesFromNoise2(map, mapGen, numCats);
                break;
            case BiomeInitialization.Painting:
                biomes = GetBiomesFromPainted2(map, mapGen, numCats);
                break;
            case BiomeInitialization.BasicWaterTemp:
                biomes = GetBiomesFromBasicWaterTemp2(map, mapGen, numCats);
                break;
            case BiomeInitialization.AdvancedWaterTemp:
                biomes = GetBiomesFromAdvancedWaterTemp2(map, mapGen, numCats);
                break;
            case BiomeInitialization.AutomaticWaterTemp:
                biomes = GetBiomesFromAutomaticWaterTemp2(map, mapGen, numCats);
                break;
            case BiomeInitialization.AutomaticWaterTempLongLat:
                biomes = GetBiomesFromAutomaticWaterTempLongLat2(map, mapGen, numCats);
                break;
        }
        return biomes;
    }
    public static float[][,] GetBiomesFromMethod(IMap map, MapGen mapGen, int numCats, BiomeInitialization biomeInit)
    {
        float[][,] biomes = new float[numCats][,];
        switch (biomeInit)
        {
            case BiomeInitialization.Clustering:
                biomes = GetBiomesFromClustering(map, mapGen, numCats);
                break;
            case BiomeInitialization.Noise:
                biomes = GetBiomesFromNoise(map, mapGen, numCats);
                break;
            case BiomeInitialization.Painting:
                biomes = GetBiomesFromPainted(map, mapGen, numCats);
                break;
            case BiomeInitialization.BasicWaterTemp:
                biomes = GetBiomesFromBasicWaterTemp(map, mapGen, numCats);
                break;
            case BiomeInitialization.AdvancedWaterTemp:
                biomes = GetBiomesFromAdvancedWaterTemp(map, mapGen, numCats);
                break;
            case BiomeInitialization.AutomaticWaterTemp:
                biomes = GetBiomesFromAutomaticWaterTemp(map, mapGen, numCats);
                break;
            case BiomeInitialization.AutomaticWaterTempLongLat:
                biomes = GetBiomesFromAutomaticWaterTempLongLat(map, mapGen, numCats);
                break;
        }
        return biomes;
    }
    public static float[][,] GetBiomesFromClustering(IMap map, MapGen mapGen, int numCats)
    {
        Clustering km = new Clustering();
        int[,] biomeClusters = km.ClusterMap(mapGen, numCats);

        float[][,] biomes = new float[numCats][,];
        for (int i = 0; i < numCats; i++)
        {
            biomes[i] = new float[map.xDim, map.yDim]; ;
        }

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                bool isAboveSeaLevel = mapGen.Elevation[x, y] > mapGen.seaLevel;
                if (isAboveSeaLevel)
                {
                    int ibiomeHere = biomeClusters[x, y];
                    biomes[ibiomeHere][x, y] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[,][] GetBiomesFromClustering2(IMap map, MapGen mapGen, int numCats)
    {
        Clustering km = new Clustering();
        int[,] biomeClusters = km.ClusterMap(mapGen, numCats);

        float[,][] biomes = new float[map.xDim,map.yDim][];

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                biomes[x, y] = new float[numCats];
                bool isAboveSeaLevel = mapGen.Elevation[x, y] > mapGen.seaLevel;
                if (isAboveSeaLevel)
                {
                    int ibiomeHere = biomeClusters[x, y];
                    biomes[x, y][ibiomeHere] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[][,] GetBiomesFromNoise(IMap map, MapGen mapGen, int numCats)
    {
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
                    bool isAboveSeaLevel = mapGen.Elevation[x, y] > mapGen.seaLevel;
                    biomes[iCat][x, y] = isAboveSeaLevel ? biomes[iCat][x, y] : 0f;
                }
            }
        }
        return biomes;
    }
    public static float[,][] GetBiomesFromNoise2(IMap map, MapGen mapGen, int numCats)
    {
        float[,][] biomes = new float[map.xDim,map.yDim][];

        for (int iCat = 0; iCat < numCats; iCat++)
        {
            ElevationBuilder elevationBuilder = new ElevationBuilder(MapUtil.nFromDims(map.xDim, map.yDim));
            elevationBuilder.SetElevationWithMidpointDisplacement(1);
            for (int x = 0; x < map.xDim; x++)
            {
                for (int y = 0; y < map.yDim; y++)
                {
                    biomes[x, y] = new float[numCats];
                    bool isAboveSeaLevel = mapGen.Elevation[x, y] > mapGen.seaLevel;
                    biomes[x, y][iCat] = isAboveSeaLevel ? elevationBuilder.Elevation[x, y] : 0f;
                }
            }
        }
        return biomes;
    }
    public static float[][,] GetBiomesFromPainted(IMap map, MapGen mapGen, int numCats)
    {
        mapGen.PaintRegions();

        float[][,] biomes = new float[numCats][,];
        for (int i = 0; i < numCats; i++)
        {
            biomes[i] = new float[map.xDim, map.yDim];
        }

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                float value = mapGen.Regions[x, y];
                int biomeVal = (int)value % numCats;
                biomes[biomeVal][x, y] = 1f;
            }
        }

        return biomes;
    }
    public static float[,][] GetBiomesFromPainted2(IMap map, MapGen mapGen, int numCats)
    {
        mapGen.PaintRegions();

        float[,][] biomes = new float[map.xDim, map.yDim][];

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                biomes[x, y] = new float[numCats];
                float value = mapGen.Regions[x, y];
                int biomeVal = (int)value % numCats;
                biomes[x, y][biomeVal] = 1f;
            }
        }

        return biomes;
    }
    public static float[][,] GetBiomesFromBasicWaterTemp(IMap map, MapGen mapGen, int numCats)
    {
        float[][,] biomes = new float[numCats][,];
        for (int i = 0; i < numCats; i++)
        {
            biomes[i] = new float[map.xDim, map.yDim];
        }

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];

                if (mapGen.Land[x, y] != 0f)
                {
                    if (water > 0.5f)
                    {
                        if (temperature > 0.5f)
                        {
                            biomes[0][x, y] = 1f;
                        }
                        else
                        {
                            biomes[1 % biomes.Length][x, y] = 1f;
                        }
                    }
                    else
                    {
                        if (temperature > 0.5f)
                        {
                            biomes[2 % biomes.Length][x, y] = 1f;
                        }
                        else
                        {
                            biomes[3 % biomes.Length][x, y] = 1f;
                        }
                    }
                }
            }
        }

        return biomes;
    }
    public static float[,][] GetBiomesFromBasicWaterTemp2(IMap map, MapGen mapGen, int numCats)
    {
        float[,][] biomes = new float[map.xDim, map.yDim][];
        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                biomes[x, y] = new float[numCats];
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];

                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeVal = 0;
                    if (water > 0.5f)
                    {
                        if (temperature > 0.5f)
                        {
                            biomeVal = 0;
                        }
                        else
                        {
                            biomeVal = 1;
                        }
                    }
                    else
                    {
                        if (temperature > 0.5f)
                        {
                            biomeVal = 2;
                        }
                        else
                        {
                            biomeVal = 3;
                        }
                    }
                    biomeVal = biomeVal % biomes.Length;
                    biomes[x, y][biomeVal] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[][,] GetBiomesFromAdvancedWaterTemp(IMap map, MapGen mapGen, int numCats)
    {
        float[][,] biomes = new float[numCats][,];
        for (int i = 0; i < numCats; i++)
        {
            biomes[i] = new float[map.xDim, map.yDim];
        }

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];

                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeIndex = 0;
                    if (water < 0.25f)
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 1 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 2 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 3 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 4 % biomes.Length;
                        }
                    }
                    else if (water < 0.5f)
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 5 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 6 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 7 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 8 % biomes.Length;
                        }
                    }
                    else if (water < 0.75f)
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 9 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 10 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 11 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 12 % biomes.Length;
                        }
                    }
                    else
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 13 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 14 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 15 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 16 % biomes.Length;
                        }
                    }
                    biomes[biomeIndex][x, y] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[,][] GetBiomesFromAdvancedWaterTemp2(IMap map, MapGen mapGen, int numCats)
    {
        float[,][] biomes = new float[map.xDim, map.yDim][];

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                biomes[x, y] = new float[numCats];
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];

                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeIndex = 0;
                    if (water < 0.25f)
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 1 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 2 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 3 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 4 % biomes.Length;
                        }
                    }
                    else if (water < 0.5f)
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 5 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 6 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 7 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 8 % biomes.Length;
                        }
                    }
                    else if (water < 0.75f)
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 9 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 10 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 11 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 12 % biomes.Length;
                        }
                    }
                    else
                    {
                        if (temperature < 0.25f)
                        {
                            biomeIndex = 13 % biomes.Length;
                        }
                        else if (temperature < 0.5f)
                        {
                            biomeIndex = 14 % biomes.Length;
                        }
                        else if (temperature < 0.75f)
                        {
                            biomeIndex = 15 % biomes.Length;
                        }
                        else
                        {
                            biomeIndex = 16 % biomes.Length;
                        }
                    }
                    biomes[x, y][biomeIndex] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[][,] GetBiomesFromAutomaticWaterTemp(IMap map, MapGen mapGen, int numCats)
    {
        float[][,] biomes = new float[numCats][,];
        for (int i = 0; i < numCats; i++)
        {
            biomes[i] = new float[map.xDim, map.yDim];
        }

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];
                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeVal = 0;
                    for (float w = 0; w < numCats; w++)
                    {
                        for (float t = 0; t < numCats; t++)
                        {
                            int k = (int)(w * numCats + t + 1f);
                            float wk = w / numCats;
                            float tk = t / numCats;
                            if (water >= wk && temperature >= tk)
                            {
                                biomeVal = k % biomes.Length;
                            }
                        }
                    }
                    biomes[biomeVal][x, y] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[,][] GetBiomesFromAutomaticWaterTemp2(IMap map, MapGen mapGen, int numCats)
    {
        float[,][] biomes = new float[map.xDim, map.yDim][];

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                biomes[x, y] = new float[numCats];
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];
                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeVal = 0;
                    for (float w = 0; w < numCats; w++)
                    {
                        for (float t = 0; t < numCats; t++)
                        {
                            int k = (int)(w * numCats + t + 1f);
                            float wk = w / numCats;
                            float tk = t / numCats;
                            if (water >= wk && temperature >= tk)
                            {
                                biomeVal = k % biomes.Length;
                            }
                        }
                    }
                    biomes[x, y][biomeVal] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[][,] GetBiomesFromAutomaticWaterTempLongLat(IMap map, MapGen mapGen, int numCats)
    {
        float[][,] biomes = new float[numCats][,];
        for (int i = 0; i < numCats; i++)
        {
            biomes[i] = new float[map.xDim, map.yDim];
        }

        int internalNumCats = 3;

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];
                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeVal = 0;

                    float longitude = x / (float)map.xDim;
                    float latitude = y / (float)map.yDim;
                    int kk = 0;
                    for (float w = 0; w < internalNumCats; w++)
                    {
                        for (float t = 0; t < internalNumCats; t++)
                        {
                            for (float lat = 0; lat < internalNumCats; lat++)
                            {
                                for (float lng = 0; lng < internalNumCats; lng++)
                                {
                                    kk++;
                                    float wkk = w / internalNumCats;
                                    float tkk = t / internalNumCats;
                                    float latk = lat / internalNumCats;
                                    float lngk = lng / internalNumCats;
                                    if (water >= wkk && temperature >= tkk && latitude >= latk && longitude >= lngk)
                                    {
                                        biomeVal = kk % biomes.Length;
                                    }
                                }
                            }
                        }
                    }
                    biomes[biomeVal][x, y] = 1f;
                }
            }
        }

        return biomes;
    }
    public static float[,][] GetBiomesFromAutomaticWaterTempLongLat2(IMap map, MapGen mapGen, int numCats)
    {
        float[,][] biomes = new float[map.xDim, map.yDim][];

        int internalNumCats = 3;

        for (int x = 0; x < map.xDim; x++)
        {
            for (int y = 0; y < map.yDim; y++)
            {
                biomes[x, y] = new float[numCats];
                float water = mapGen.WaterFlux[x, y];
                float temperature = mapGen.Temperature[x, y];
                if (mapGen.Land[x, y] != 0f)
                {
                    int biomeVal = 0;

                    float longitude = x / (float)map.xDim;
                    float latitude = y / (float)map.yDim;
                    int kk = 0;
                    for (float w = 0; w < internalNumCats; w++)
                    {
                        for (float t = 0; t < internalNumCats; t++)
                        {
                            for (float lat = 0; lat < internalNumCats; lat++)
                            {
                                for (float lng = 0; lng < internalNumCats; lng++)
                                {
                                    kk++;
                                    float wkk = w / internalNumCats;
                                    float tkk = t / internalNumCats;
                                    float latk = lat / internalNumCats;
                                    float lngk = lng / internalNumCats;
                                    if (water >= wkk && temperature >= tkk && latitude >= latk && longitude >= lngk)
                                    {
                                        biomeVal = kk % biomes.Length;
                                    }
                                }
                            }
                        }
                    }
                    biomes[x, y][biomeVal] = 1f;
                }
            }
        }

        return biomes;
    }
}
