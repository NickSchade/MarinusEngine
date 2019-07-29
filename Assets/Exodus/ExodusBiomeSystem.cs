using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ExodusBiomeSystem
{
    public int _numberOfBiomes { get; set; }
    public Color[] biomeColors { get; set; }
    public int currentBiome { get; set; }
    public ExodusLocationColorType _currentColorType;

    public ExodusBiomeSystem(int numberOfBiomes)
    {
        _numberOfBiomes = numberOfBiomes;
        currentBiome = 0;

        _currentColorType = ExodusLocationColorType.DiscreteSingle;
        biomeColors = Colors.ColorListProcedural(ExodusSettings.numberOfBiomes);
        biomeColors = Colors.ColorListManual();
    }
    public void RotateBiome()
    {
        int newBiome = currentBiome == _numberOfBiomes - 1 ? 0 : currentBiome + 1;
        Debug.Log("Rotating Biome from " + currentBiome.ToString() + " to " + newBiome.ToString());
        currentBiome = newBiome;
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


    public Color GetColorFromType(ExodusLocation location, Color currentColor, ExodusLocationColorType type)
    {
        Color c = currentColor;

        switch (type)
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
        if (location._biomes[currentBiome] > ExodusSettings.biomeCutoffDiscrete)
        {
            currentColor = biomeColors[currentBiome];
        }

        return currentColor;
    }

    private Color GetColorDiscreteManyBiome(Color currentColor, ExodusLocation location)
    {
        int maxIndex = location.GetMaxBiome();
        if (location._biomes[maxIndex] > ExodusSettings.biomeCutoffDiscrete)
        {
            currentColor = biomeColors[maxIndex];
        }
        return currentColor;
    }

    private Color GetColorContinuousSingleBiome(Color currentColor, ExodusLocation location)
    {
        if (location._biomes[currentBiome] > ExodusSettings.biomeCutoffContinuous)
        {
            Color pureColor = biomeColors[currentBiome];
            currentColor = MapColor.GetColorLerp(location._biomes[currentBiome], Color.white, pureColor);
        }

        return currentColor;
    }

    private Color GetColorContinuousManyBiome(Color currentColor, ExodusLocation location)
    {
        int maxIndex = location.GetMaxBiome();
        if (location._biomes[maxIndex] > ExodusSettings.biomeCutoffContinuous)
        {
            Color pureColor = biomeColors[maxIndex];
            currentColor = MapColor.GetColorLerp(location._biomes[maxIndex], Color.white, pureColor);
        }

        return currentColor;
    }
}
