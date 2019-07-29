using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExodusLocationColorType { DiscreteSingle, DiscreteMulti, ContinuousSingle, ContinuousMulti };
public class ExodusLocationColor : ILocationColor
{
    ExodusBiomeSystem _biomeSystem;
    public ExodusLocationColor(ExodusBiomeSystem biomeSystem)
    {
        _biomeSystem = biomeSystem;
    }
    public Color GetColor(ExodusLocation exodusLocation)
    {
        Color c = SetDefaultColor(exodusLocation);
        c = _biomeSystem.GetColor(exodusLocation, c);
        return c;
    }
    public Color GetColor(ILocation location)
    {
        return Color.black;
    }
    private Color SetDefaultColor(ExodusLocation location)
    {
        Color c = Color.magenta;
        if (location._qualities["Land"] == 0f)
        {
            c = Color.black;
        }
        else
        {
            c = Color.white;
        }
        return c;
    }
}
