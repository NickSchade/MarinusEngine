using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ExodusBiomeSystem
{
    public int _numberOfBiomes { get; set; }
    public Color[] _biomeColors { get; set; }
    public int _currentBiome { get; set; }
    public ExodusLocationColorType _currentColorType;
    public float[,][] _biomes;

    public ExodusBiomeSystem(int numberOfBiomes)
    {
        InitializeExodusBiomeSystem(numberOfBiomes);
    }
    public ExodusBiomeSystem(IMap map, MapGen mapGen, int numberOfBiomes)
    {
        InitializeExodusBiomeSystem(numberOfBiomes);
        _biomes = ExodusBiomeInitializer.GetBiomes(map, mapGen, numberOfBiomes, BiomeInitialization.Noise);
    }
    private void InitializeExodusBiomeSystem(int numberOfBiomes)
    {

        _numberOfBiomes = numberOfBiomes;
        _currentBiome = 0;

        _currentColorType = ExodusLocationColorType.DiscreteSingle;
        _biomeColors = Colors.ColorListProcedural(ExodusSettings.numberOfBiomes);
        _biomeColors = Colors.ColorListManual();
    }
    public void RotateBiome()
    {
        int newBiome = _currentBiome == _numberOfBiomes - 1 ? 0 : _currentBiome + 1;
        Debug.Log("Rotating Biome from " + _currentBiome.ToString() + " to " + newBiome.ToString());
        _currentBiome = newBiome;
    }
    public void RotateView()
    {
        ExodusLocationColorType newType = _currentColorType;
        if (_currentColorType == ExodusLocationColorType.DiscreteSingle)
        {
            newType = ExodusLocationColorType.DiscreteMulti;
        }
        else if (_currentColorType == ExodusLocationColorType.DiscreteMulti)
        {
            newType = ExodusLocationColorType.ContinuousSingle;
        }
        else if (_currentColorType == ExodusLocationColorType.ContinuousSingle)
        {
            newType = ExodusLocationColorType.ContinuousMulti;
        }
        else if (_currentColorType == ExodusLocationColorType.ContinuousMulti)
        {
            newType = ExodusLocationColorType.DiscreteSingle;
        }
        Debug.Log("Rotating from " + _currentColorType.ToString() + " to " + newType.ToString());
        _currentColorType = newType;
    }


    public Color GetColor(ExodusLocation location, Color currentColor)
    {
        Color c = currentColor;

        switch (_currentColorType)
        {
            case ExodusLocationColorType.DiscreteSingle:
                c = GetColorDiscreteSingleBiome(c, location);
                break;
            case ExodusLocationColorType.DiscreteMulti:
                c = GetColorDiscreteManyBiome(c, location);
                break;
            case ExodusLocationColorType.ContinuousSingle:
                c = GetColorContinuousSingleBiome(c, location);
                break;
            case ExodusLocationColorType.ContinuousMulti:
                c = GetColorContinuousManyBiome(c, location);
                break;
        }

        return c;
    }

    private Color GetColorDiscreteSingleBiome(Color currentColor, ExodusLocation location)
    {
        if (location._biomes[_currentBiome] > ExodusSettings.biomeCutoffDiscrete)
        {
            currentColor = _biomeColors[_currentBiome];
        }

        return currentColor;
    }

    private Color GetColorDiscreteManyBiome(Color currentColor, ExodusLocation location)
    {
        int maxIndex = location.GetMaxBiome();
        if (location._biomes[maxIndex] > ExodusSettings.biomeCutoffDiscrete)
        {
            currentColor = _biomeColors[maxIndex];
        }
        return currentColor;
    }

    private Color GetColorContinuousSingleBiome(Color currentColor, ExodusLocation location)
    {
        if (location._biomes[_currentBiome] > ExodusSettings.biomeCutoffContinuous)
        {
            Color pureColor = _biomeColors[_currentBiome];
            currentColor = MapColor.GetColorLerp(location._biomes[_currentBiome], Color.white, pureColor);
        }

        return currentColor;
    }

    private Color GetColorContinuousManyBiome(Color currentColor, ExodusLocation location)
    {
        int maxIndex = location.GetMaxBiome();
        if (location._biomes[maxIndex] > ExodusSettings.biomeCutoffContinuous)
        {
            Color pureColor = _biomeColors[maxIndex];
            currentColor = MapColor.GetColorLerp(location._biomes[maxIndex], Color.white, pureColor);
        }

        return currentColor;
    }
}
