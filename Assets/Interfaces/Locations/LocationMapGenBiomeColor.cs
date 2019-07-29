using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMapGenBiomeColor : LocationAbstract, ILocation
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
