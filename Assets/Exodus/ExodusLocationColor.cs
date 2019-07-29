using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExodusLocationColorType { DiscreteSingle, DiscreteMulti, ContinuousSingle, ContinuousMulti };
public class ExodusLocationColor : ILocationColor
{
    IGame _game;
    public ExodusLocationColor(IGame game)
    {
        _game = game;
    }
    public Color GetColor(ExodusLocation exodusLocation)
    {
        Color c = SetDefaultColor(exodusLocation);
        c = _game._biomeSystem.GetColorFromType(exodusLocation, c, _game._biomeSystem._currentColorType);
        return c;
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
